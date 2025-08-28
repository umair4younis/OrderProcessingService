using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using OrderProcessingService.Application.DTOs;
using System.Net.Http.Json;

namespace OrderProcessingService.Tests.Integration;

public class OrdersApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public OrdersApiTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task Create_then_get_order()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-API-KEY", "local-dev-key");

        var request = new CreateOrderRequest
        {
            CustomerName = "Alice",
            Items = { new CreateOrderItemDto { ProductId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Quantity = 1 } }
        };

        var create = await client.PostAsJsonAsync("/orders", request);
        create.EnsureSuccessStatusCode();
        var created = await create.Content.ReadFromJsonAsync<OrderDto>();
        created.Should().NotBeNull();

        var get = await client.GetAsync($"/orders/{created!.Id}");
        get.EnsureSuccessStatusCode();
        var fetched = await get.Content.ReadFromJsonAsync<OrderDto>();
        fetched!.Id.Should().Be(created.Id);
    }
}