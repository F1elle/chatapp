using System.Text;
using ChatApp.User.Common.Extensions;
using ChatApp.User.Common.Middleware;
using ChatApp.User.Infrastructure.Data;
using ChatApp.User.Infrastructure.Messaging;
using ChatApp.User.Infrastructure.Messaging.Handlers;
using ChatApp.User.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using Rebus.Config;
using Rebus.Retry.Simple;

namespace ChatApp.User;

public static class ConfigureServices
{
    public static void ConfigureAppServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(configure =>
        {
            configure.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
        builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));

        var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.TokenValidationParameters = new()
                        {
                            ValidateIssuer = true,
                            ValidIssuer = jwtOptions!.Issuer,
                            ValidateAudience = true,
                            ValidAudience = jwtOptions!.Audience,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions!.Secret))
                        };
                    });

        builder.Services.AddAuthorization();

        builder.Services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null
                    );
                    npgsqlOptions.CommandTimeout(30);
                }
            ));

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddHandlers();

        var rabbitMqOptions = builder.Configuration
            .GetSection(RabbitMqOptions.SectionName)
            .Get<RabbitMqOptions>();

        builder.Services.AutoRegisterHandlersFromAssemblyOf<UserSignedUpHandler>();

        builder.Services.AddRebus(configure => configure
            .Transport(t => t.UseRabbitMq(
                connectionString: rabbitMqOptions!.ConnectionString,
                inputQueueName: rabbitMqOptions!.InputQueueName
            ))
            .Options(o =>
            {
                o.RetryStrategy(maxDeliveryAttempts: 3, secondLevelRetriesEnabled: true);

                o.SetNumberOfWorkers(1);
                o.SetMaxParallelism(1);
            }));

        builder.Services.AddSingleton<IConnection>(sp =>
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(rabbitMqOptions!.ConnectionString)
                };
                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            });

        builder.Services.AddHealthChecks()
            .AddNpgSql(
                connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
                name: "UserDbContext",
                timeout: TimeSpan.FromSeconds(5),
                tags: new[] { "db", "postgresql" }
            )
            .AddRabbitMQ(
                name: "RabbitMQ",
                timeout: TimeSpan.FromSeconds(5),
                tags: new[] { "messaging", "rabbitmq" }
            );
    }
}