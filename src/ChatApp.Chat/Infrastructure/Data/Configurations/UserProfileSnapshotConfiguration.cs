using ChatApp.Chat.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Chat.Infrastructure.Data.Configurations;

public class UserProfileSnapshotConfiguration : IEntityTypeConfiguration<UserProfileSnapshot>
{
    public void Configure(EntityTypeBuilder<UserProfileSnapshot> builder)
    {
        builder.ToTable("user_profile_snapshot");

        builder.HasKey(ups => ups.UserId);

        builder.Property(ups => ups.DisplayName).IsRequired().HasMaxLength(80);

        builder.Property(ups => ups.SyncedAt).IsRequired().HasDefaultValue("CURRENT_TIMESTAMP");
    }
}
