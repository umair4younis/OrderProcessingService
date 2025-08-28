using OrderProcessingService.Domain.Enums;

namespace OrderProcessingService.Application.DTOs;
public record OrderDto(Guid Id, string CustomerName, DateTime CreatedAtUtc, OrderStatus Status, List<OrderItemDto> Items);
public record OrderItemDto(Guid ProductId, int Quantity);