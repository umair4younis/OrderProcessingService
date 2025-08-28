namespace OrderProcessingService.Application.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<T>(string topic, T payload, CancellationToken ct);
}