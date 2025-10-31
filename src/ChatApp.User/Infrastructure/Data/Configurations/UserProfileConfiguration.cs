using ChatApp.User.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.User.Infrastructure.Data.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
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

        builder.HasMany(up => up.Contacts)
            .WithMany();
    }
}