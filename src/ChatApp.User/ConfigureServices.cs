using System.Text;
using ChatApp.User.Common.Middleware;
using ChatApp.User.Features.UserProfile.CreateUserProfile;
using ChatApp.User.Infrastructure.Data;
using ChatApp.User.Infrastructure.Messaging;
using ChatApp.User.Infrastructure.Messaging.Handlers;
using ChatApp.User.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        builder.Services.AddScoped<CreateUserProfileHandler>();

        var rabbitMqOptions = builder.Configuration
            .GetSection(RabbitMqOptions.SectionName)
            .Get<RabbitMqOptions>();

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

        builder.Services.AutoRegisterHandlersFromAssemblyOf<UserSignedUpHandler>();

    }
}