using ChatApp.Auth.Domain;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Auth.Infrastructure.Data;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<UserAuth> UserAuth { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}