using Microsoft.EntityFrameworkCore;
using OrderProcessingService.Application.DTOs;
using OrderProcessingService.Application.Interfaces;
using OrderProcessingService.Domain.Entities;
using OrderProcessingService.Domain.Enums;
using OrderProcessingService.Infrastructure;

namespace OrderProcessingService.Application.Services;
public class OrderService : IOrderService
{
    private readonly AppDbContext _db;
    private readonly IEventPublisher _publisher;
    public OrderService(AppDbContext db, IEventPublisher publisher)
    {
        _db = db;
        _publisher = publisher;
    }

    public async Task<OrderDto> PlaceOrderAsync(CreateOrderRequest request, CancellationToken ct)
    {
        // basic validation is handled by data annotations; ensure products exist
        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _db.Products.Where(p => productIds.Contains(p.Id)).ToListAsync(ct);
        if (products.Count != productIds.Count)
        {
            throw new ArgumentException("One or more products do not exist.");
        }

        var order = new Order
        {
            CustomerName = request.CustomerName,
            Items = request.Items.Select(i => new OrderItem { ProductId = i.ProductId, Quantity = i.Quantity }).ToList()
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct);

        await _publisher.PublishAsync("order.created", new { order.Id, order.CustomerName }, ct);

        return await MapAsync(order.Id, ct);
    }

    public async Task<OrderDto?> GetOrderAsync(Guid id, CancellationToken ct)
    {
        var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id, ct);
        return order is null ? null : Map(order);
    }

    public async Task<OrderStatus?> GetStatusAsync(Guid id, CancellationToken ct)
    {
        var order = await _db.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id, ct);
        return order?.Status;
    }

    public async Task<bool> ConfirmOrderAsync(Guid id, CancellationToken ct)
    {
        // Use a transaction to ensure stock is reserved atomically
        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id, ct);
        if (order is null || order.Status != OrderStatus.Pending) return false;

        // Load products with rowversion for optimistic concurrency
        var productIds = order.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _db.Products.Where(p => productIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, ct);

        // Check availability
        foreach (var item in order.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var product)) return false;
            if (product.StockQuantity < item.Quantity) return false;
        }

        // Reserve stock
        foreach (var item in order.Items)
        {
            var product = products[item.ProductId];
            product.StockQuantity -= item.Quantity;
            _db.Products.Update(product);
        }

        order.Status = OrderStatus.Confirmed;
        await _db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        await _publisher.PublishAsync("order.confirmed", new { order.Id }, ct);
        return true;
    }

    public async Task<bool> ShipOrderAsync(Guid id, CancellationToken ct)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);
        if (order is null || order.Status != OrderStatus.Confirmed) return False();
        order.Status = OrderStatus.Shipped;
        await _db.SaveChangesAsync(ct);
        await _publisher.PublishAsync("order.shipped", new { order.Id }, ct);
        return true;

        static bool False() => false;
    }

    private async Task<OrderDto> MapAsync(Guid id, CancellationToken ct)
    {
        var order = await _db.Orders.Include(o => o.Items).FirstAsync(o => o.Id == id, ct);
        return Map(order);
    }
    private static OrderDto Map(Order order)
        => new(order.Id, order.CustomerName, order.CreatedAtUtc, order.Status,
               order.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity)).ToList());
}