using System.Text;
using ChatApp.Auth.Common.Extensions;
using ChatApp.Auth.Common.Middleware;
using ChatApp.Auth.Infrastructure.Data;
using ChatApp.Auth.Infrastructure.Messaging;
using ChatApp.Auth.Infrastructure.Security;
using ChatApp.Common.Infrastructure.Messaging.Events;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using Rebus.Config;
using Rebus.Routing.TypeBased;

namespace ChatApp.Auth;

public static class ConfigureServices
{
    public static async Task ConfigureAppServices(this WebApplicationBuilder builder)
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


        builder.Services.AddDbContext<AuthDbContext>(options =>
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
                }));


        builder.Services.AddHttpContextAccessor();


        builder.Services.AddSingleton<TokenProvider>();
        builder.Services.AddSingleton<PasswordHasher>();

        builder.Services.AddHandlers();


        var rabbitMqOptions = builder.Configuration
            .GetSection(RabbitMqOptions.SectionName)
            .Get<RabbitMqOptions>();

        builder.Services.AutoRegisterHandlersFromAssemblyOf<Program>();

        builder.Services.AddRebus(configure => configure
            .Logging(l => l.Console(minLevel: Rebus.Logging.LogLevel.Info))
            .Transport(t => t.UseRabbitMqAsOneWayClient(
                connectionString: rabbitMqOptions!.ConnectionString
            ))
            .Routing(r => r.TypeBased().Map<UserSignedUpEvent>(rabbitMqOptions!.Routing))
            .Options(o =>
            {
                o.SetMaxParallelism(1);
                o.SetNumberOfWorkers(1);
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
                connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!, // TODO: remove !
                name: "AuthDbContext",
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