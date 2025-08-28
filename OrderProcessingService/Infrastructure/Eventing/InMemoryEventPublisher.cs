using System.Text.Json;
using OrderProcessingService.Application.Interfaces;

namespace OrderProcessingService.Infrastructure.Eventing;
public class InMemoryEventPublisher : IEventPublisher
{
    public Task PublishAsync<T>(string topic, T payload, CancellationToken ct)
    {
        // For POC we just log to console; in production this could be Kafka, RabbitMQ, etc.
        Console.WriteLine($"EVENT [{topic}] {JsonSerializer.Serialize(payload)}");
        return Task.CompletedTask;
    }
}