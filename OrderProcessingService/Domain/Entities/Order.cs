using OrderProcessingService.Domain.Enums;

namespace OrderProcessingService.Domain.Entities;
public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CustomerName { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public List<OrderItem> Items { get; set; } = new();
}