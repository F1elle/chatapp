using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Chat.Infrastructure.Data.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<Domain.Chat>
{
    public void Configure(EntityTypeBuilder<Domain.Chat> builder)
    {
        builder.ToTable("chats");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Type)
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(256);

        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasMany(c => c.ChatParticipants)
            .WithOne()
            .HasForeignKey(cp => cp.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Messages)
            .WithOne()
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.LastMessage)
            .WithMany()
            .HasForeignKey(c => c.LastMessageId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}