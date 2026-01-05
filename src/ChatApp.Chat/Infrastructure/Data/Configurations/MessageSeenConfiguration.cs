using ChatApp.Chat.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Chat.Infrastructure.Data.Configurations;

public class MessageSeenConfiguration : IEntityTypeConfiguration<MessageSeen>
{
    public void Configure(EntityTypeBuilder<MessageSeen> builder)
    {
        builder.ToTable("message_participant_seen");

        builder.HasKey(ms => ms.Id);

        builder.HasIndex(ms => new { ms.ParticipantId, ms.MessageId});

        builder.Property(ms => ms.SeenAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne<Message>()
            .WithMany(m => m.SeenByParticipants)
            .HasForeignKey(ms => ms.MessageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ChatParticipant>()
            .WithMany() // TODO: add navigation property when needed 
            .HasForeignKey(ms => ms.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade); // TODO: maybe move to another entity
    }
}