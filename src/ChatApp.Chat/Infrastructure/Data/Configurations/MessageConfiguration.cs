using ChatApp.Chat.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Chat.Infrastructure.Data.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.SentAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(m => m.Content)
            .HasMaxLength(4096);
        builder.Property(m => m.Type)
            .IsRequired();
        

        builder.Property(m => m.AttachmentIds).HasColumnType("uuid[]");

        builder.HasOne(m => m.ReplyToMessage)
            .WithMany(m => m.Replies)
            .HasForeignKey(m => m.ReplyToMessageId)
            .OnDelete(DeleteBehavior.SetNull);
        

        builder.Ignore(m => m.IsEdited);
        builder.Ignore(m => m.IsRead);
        builder.Ignore(m => m.RepliesCount);
        builder.Ignore(m => m.SeenCount);
    }
}