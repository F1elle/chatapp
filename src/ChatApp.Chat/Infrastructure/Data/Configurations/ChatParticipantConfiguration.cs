using ChatApp.Chat.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Chat.Infrastructure.Data.Configurations;

public class ChatParticipantConfiguration : IEntityTypeConfiguration<ChatParticipant>
{
    public void Configure(EntityTypeBuilder<ChatParticipant> builder)
    {
        builder.ToTable("chat_participants");

        builder.HasKey(m => m.Id);

        // builder.HasIndex(cp => new { cp.ChatId, cp.UserId }).IsUnique();

        builder.Property(cp => cp.JoinedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder
            .HasOne(cp => cp.Chat)
            .WithMany(c => c.Participants)
            .HasForeignKey(cp => cp.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        // explicitly no userId
    }
}
