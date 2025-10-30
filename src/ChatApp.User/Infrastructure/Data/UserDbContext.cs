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
        builder.Property(up => up.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.HasIndex(up => up.UserTag)
            .IsUnique()
            .HasDatabaseName("idx_user_tag");

        builder.Property(up => up.DisplayName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(up => up.UserTag).HasMaxLength(40);

        builder.Property(up => up.Bio).HasMaxLength(250);
    }

    public DbSet<UserProfile> UserProfiles { get; set; }
}