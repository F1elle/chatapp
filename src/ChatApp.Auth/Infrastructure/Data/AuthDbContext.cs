using ChatApp.Auth.Domain;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Auth.Infrastructure.Data;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureUserAuthTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<UserAuthInfo>();

        builder.ToTable("user_auth");

        builder.HasKey(ua => ua.Id);
        builder.HasIndex(ua => ua.Email).IsUnique();
        builder.HasIndex(ua => ua.UserName).IsUnique();
        builder.Property(ua => ua.PasswordHash).IsRequired(); // TODO: max length
    }

    public DbSet<UserAuthInfo> UserAuth { get; set; }
}