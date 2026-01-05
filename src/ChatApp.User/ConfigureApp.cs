using System.Text.Json;
using ChatApp.User.Features;
using ChatApp.User.Infrastructure.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.User;

public static class ConfigureApp
{
    public static async Task Configure(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapUserEndpoints();

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    results = report.Entries.Select(e => new
                    {
                        key = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description
                    })
                });
                await context.Response.WriteAsync(result);
            }
        });

        await app.MigrateDb();
    }

    private static async Task MigrateDb(this WebApplication app)
    {
        using var serviceScope = app.Services.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<UserDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}