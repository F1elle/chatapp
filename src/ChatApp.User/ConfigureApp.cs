using ChatApp.User.Features;
using ChatApp.User.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.User;

public static class ConfigureApp
{
    public static async Task Configure(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.MapUserEndpoints();
        app.UseAuthentication();
        app.UseAuthorization();

        await app.MigrateDb();
    }

    private static async Task MigrateDb(this WebApplication app)
    {
        using var serviceScope = app.Services.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<UserDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}