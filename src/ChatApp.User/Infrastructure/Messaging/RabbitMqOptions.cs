namespace ChatApp.User.Infrastructure.Messaging;

public record RabbitMqOptions
{
    public const string SectionName = "RabbitMQ";
    
    public required string ConnectionString { get; init; }
    public required string InputQueueName { get; init; }
    public required string Routing { get; init; }
}