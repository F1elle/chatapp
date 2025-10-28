using ChatApp.Auth.Features;
using ChatApp.Auth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Auth;

public static class ConfigureApp
{
    public static async Task Configure(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.MapAuthEndpoints();
        app.UseAuthentication();
        app.UseAuthorization();

        await app.MigrateDb();
    }

    private static async Task MigrateDb(this WebApplication app)
    {
        using var serviceScope = app.Services.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<AuthDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}