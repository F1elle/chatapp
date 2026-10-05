using Microsoft.EntityFrameworkCore;

namespace ChatApp.Common.Infrastructure.Outbox.Extensions;

public static class OutboxModelBuilderExtensions
{
    public static void ApplyOutboxConfiguration(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(builder =>
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Type).IsRequired().HasMaxLength(200);
            builder.HasIndex(m => m.ProcessedAt);
        });
    }
}
