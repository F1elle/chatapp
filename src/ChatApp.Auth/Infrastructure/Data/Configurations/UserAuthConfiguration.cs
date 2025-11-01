using ChatApp.Auth.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Auth.Infrastructure.Data.Configurations;

public class UserAuthConfiguration : IEntityTypeConfiguration<UserAuth>
{
    public void Configure(EntityTypeBuilder<UserAuth> builder)
    {
        builder.ToTable("user_auth");
        
        builder.HasKey(ua => ua.Id);
        
        builder.HasIndex(ua => ua.Email)
            .IsUnique()
            .HasDatabaseName("idx_user_auth_email");

        builder.Property(ua => ua.Id)
            .IsRequired();
        
        builder.Property(ua => ua.Email)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(ua => ua.PasswordHash)
            .IsRequired()
            .HasMaxLength(128);
        
        builder.Property(ua => ua.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.HasMany(ua => ua.RefreshTokens)
            .WithOne()
            .HasForeignKey(rt => rt.UserAuthId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}