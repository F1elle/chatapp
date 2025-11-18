using ChatApp.Chat.Domain;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Chat.Infrastructure.Data;

public class ChatDbContext(DbContextOptions<ChatDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ChatDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<ChatParticipant> ChatParticipants { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Domain.Chat> Chats { get; set; }
    public DbSet<MessageSeen> MessageSeens { get; set; }
}