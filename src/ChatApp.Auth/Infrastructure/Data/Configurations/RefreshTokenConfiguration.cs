using ChatApp.Auth.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Auth.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");
        
        builder.HasKey(rt => rt.Id);
        
        builder.HasIndex(rt => rt.Token)
            .IsUnique()
            .HasDatabaseName("idx_refresh_tokens_token");
        
        builder.HasIndex(rt => rt.UserAuthId)
            .HasDatabaseName("idx_refresh_tokens_user_id");
        
        builder.HasIndex(rt => rt.ExpiresAt)
            .HasDatabaseName("idx_refresh_tokens_expires_at");  
        
        builder.Property(rt => rt.Id)
            .IsRequired()
            .ValueGeneratedNever();
        
        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();
        
        builder.Property(rt => rt.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.Property(rt => rt.CreatedByIp)
            .HasMaxLength(45);  // IPv6 max length
        
        builder.Property(rt => rt.RevokedByIp)
            .HasMaxLength(45);
        
        builder.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(200);
    }
}