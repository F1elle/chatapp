using ChatApp.User.Domain;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.User.Infrastructure.Data;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUserProfilesTable(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }


    private static void ConfigureUserProfilesTable(ModelBuilder modelBuilder)
    {
        var builder = modelBuilder.Entity<UserProfile>();

        builder.ToTable("user_profiles");

        builder.HasKey(up => up.Id);
        builder.HasIndex(up => up.UserName).IsUnique();
        builder.Property(up => up.FirstName).HasMaxLength(50);
        builder.Property(up => up.LastName).HasMaxLength(50);
        builder.Property(up => up.Bio).HasMaxLength(250);
    }

    public DbSet<UserProfile> UserProfiles { get; set; }
}