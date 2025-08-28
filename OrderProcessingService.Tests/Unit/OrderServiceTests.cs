using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrderProcessingService.Application.DTOs;
using OrderProcessingService.Application.Interfaces;
using OrderProcessingService.Application.Services;
using OrderProcessingService.Domain.Entities;
using OrderProcessingService.Domain.Enums;
using OrderProcessingService.Infrastructure;
using OrderProcessingService.Infrastructure.Eventing;

namespace OrderProcessingService.Tests.Unit;

public class OrderServiceTests
{
    private static AppDbContext CreateDb()
    {
        var opt = new DbContextOptionsBuilder<AppDbContext>().UseSqlite("Data Source=:memory:").Options;
        var db = new AppDbContext(opt);
        db.Database.OpenConnection();
        db.Database.EnsureCreated();
        return db;
    }

    [Fact]
    public async Task Place_and_confirm_order_success()
    {
        using var db = CreateDb();
        db.Products.Add(new Product { Id = Guid.NewGuid(), Name = "P1", StockQuantity = 5 });
        await db.SaveChangesAsync();

        IEventPublisher pub = new InMemoryEventPublisher();
        var svc = new OrderService(db, pub);

        var pid = await db.Products.Select(p => p.Id).FirstAsync();
        var order = await svc.PlaceOrderAsync(new CreateOrderRequest
        {
            CustomerName = "John",
            Items = { new CreateOrderItemDto { ProductId = pid, Quantity = 2 } }
        }, CancellationToken.None);

        (await svc.ConfirmOrderAsync(order.Id, CancellationToken.None)).Should().BeTrue();

        var status = await svc.GetStatusAsync(order.Id, CancellationToken.None);
        status.Should().Be(OrderStatus.Confirmed);
    }
}