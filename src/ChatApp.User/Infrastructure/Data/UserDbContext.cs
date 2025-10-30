using ChatApp.User.Domain;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.User.Infrastructure.Data;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<UserProfile> UserProfiles { get; set; }
}