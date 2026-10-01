using ChatApp.Chat.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Chat.Infrastructure.Data.Configurations;

public class DirectChatLookupConfiguration : IEntityTypeConfiguration<DirectChatLookup>
{
    public void Configure(EntityTypeBuilder<DirectChatLookup> builder)
    {
        builder.ToTable("direct_chat_lookup");

        builder.HasKey(cl => cl.ChatId);

        builder.Property(cl => cl.UserIdHigh).IsRequired();

        builder.Property(cl => cl.UserIdLow).IsRequired();
    }
}
