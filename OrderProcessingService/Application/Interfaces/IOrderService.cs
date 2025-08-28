using OrderProcessingService.Application.DTOs;
using OrderProcessingService.Domain.Enums;

namespace OrderProcessingService.Application.Interfaces;
public interface IOrderService
{
    Task<OrderDto> PlaceOrderAsync(CreateOrderRequest request, CancellationToken ct);
    Task<OrderDto?> GetOrderAsync(Guid id, CancellationToken ct);
    Task<bool> ConfirmOrderAsync(Guid id, CancellationToken ct);
    Task<bool> ShipOrderAsync(Guid id, CancellationToken ct);
    Task<OrderStatus?> GetStatusAsync(Guid id, CancellationToken ct);
}