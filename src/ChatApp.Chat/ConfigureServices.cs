using System.Text;
using ChatApp.Auth.Common.Extensions;
using ChatApp.Chat.Common.Middleware;
using ChatApp.Chat.Features.Abstractions;
using ChatApp.Chat.Infrastructure.Data;
using ChatApp.Chat.Infrastructure.Redis;
using ChatApp.Chat.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace ChatApp.Chat;

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

        builder.Services.AddDbContext<ChatDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                    npgsqlOptions.CommandTimeout(30);
                }));

        builder.Services.AddHttpContextAccessor();

        // TODO: register my services here

        builder.Services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(
                builder.Configuration.GetConnectionString("Redis")!));

        builder.Services.AddScoped<IChatPresenceService, RedisChatPresenceService>();
        builder.Services.AddScoped<IChatAccessService, ChatAccessService>();

        builder.Services.AddHandlers();

        // TODO: left here

        builder.Services.AddHealthChecks()
            .AddNpgSql(
                connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
                name: "ChatDbContext",
                timeout: TimeSpan.FromSeconds(5),
                tags: new[] { "db", "postgresql" }  
            )
            .AddRedis(
                redisConnectionString: builder.Configuration.GetConnectionString("Redis")!,
                name: "Redis",
                timeout: TimeSpan.FromSeconds(5),
                tags: new[] { "db", "redis" }
            );

        builder.Services.AddSignalR();
    }
}