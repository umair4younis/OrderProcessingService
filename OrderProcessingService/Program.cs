using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderProcessingService.Application.DTOs;
using OrderProcessingService.Application.Interfaces;
using OrderProcessingService.Application.Services;
using OrderProcessingService.Infrastructure;
using OrderProcessingService.Infrastructure.Eventing;

var builder = WebApplication.CreateBuilder(args);

// Config
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Processing API", Version = "v1" });
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-API-KEY",
        Type = SecuritySchemeType.ApiKey,
        Description = "Simple API Key for demo"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" } }, new string[] {} }
    });
});

// Data
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    var cs = builder.Configuration.GetConnectionString("Default") ?? "Data Source=orders.db";
    opt.UseSqlite(cs);
});

// Services
builder.Services.AddSingleton<IEventPublisher, InMemoryEventPublisher>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Simple API Key auth
builder.Services.AddAuthentication().AddJwtBearer(); // placeholder
builder.Services.AddAuthorization(options => options.AddPolicy("ApiKeyPolicy", policy => policy.Requirements.Add(new ApiKeyRequirement())));
builder.Services.AddSingleton<IAuthorizationHandler, ApiKeyAuthorizationHandler>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger"));

// Orders Endpoints
app.MapPost("/orders", async (CreateOrderRequest req, IOrderService svc, HttpContext ctx, CancellationToken ct) =>
{
    if (!ctx.Request.Headers.TryGetValue("X-API-KEY", out var key) || key != "local-dev-key")
        return Results.Unauthorized();

    var order = await svc.PlaceOrderAsync(req, ct);
    return Results.Created($"/orders/{order.Id}", order);
})
.WithName("PlaceOrder")
.Produces<OrderDto>(201);

app.MapGet("/orders/{id:guid}", async (Guid id, IOrderService svc, HttpContext ctx, CancellationToken ct) =>
{
    if (!ctx.Request.Headers.TryGetValue("X-API-KEY", out var key) || key != "local-dev-key")
        return Results.Unauthorized();

    var order = await svc.GetOrderAsync(id, ct);
    return order is null ? Results.NotFound() : Results.Ok(order);
})
.WithName("GetOrder")
.Produces<OrderDto>(200)
.Produces(404);

app.MapGet("/orders/{id:guid}/status", async (Guid id, IOrderService svc, HttpContext ctx, CancellationToken ct) =>
{
    if (!ctx.Request.Headers.TryGetValue("X-API-KEY", out var key) || key != "local-dev-key")
        return Results.Unauthorized();

    var status = await svc.GetStatusAsync(id, ct);
    return status is null ? Results.NotFound() : Results.Ok(status.ToString());
})
.WithName("GetOrderStatus")
.Produces<string>(200)
.Produces(404);

app.MapPost("/orders/{id:guid}/confirm", async (Guid id, IOrderService svc, HttpContext ctx, CancellationToken ct) =>
{
    if (!ctx.Request.Headers.TryGetValue("X-API-KEY", out var key) || key != "local-dev-key")
        return Results.Unauthorized();

    var ok = await svc.ConfirmOrderAsync(id, ct);
    return ok ? Results.Ok(new { Message = "Order confirmed" }) : Results.BadRequest(new { Message = "Cannot confirm (insufficient stock or invalid state)" });
})
.WithName("ConfirmOrder");

app.MapPost("/orders/{id:guid}/ship", async (Guid id, IOrderService svc, HttpContext ctx, CancellationToken ct) =>
{
    if (!ctx.Request.Headers.TryGetValue("X-API-KEY", out var key) || key != "local-dev-key")
        return Results.Unauthorized();

    var ok = await svc.ShipOrderAsync(id, ct);
    return ok ? Results.Ok(new { Message = "Order shipped" }) : Results.BadRequest(new { Message = "Cannot ship (not confirmed or missing)" });
})
.WithName("ShipOrder");

app.Run();

// Make Program visible to WebApplicationFactory for integration tests
public partial class Program { }

// Simple API key auth bits
public class ApiKeyRequirement : IAuthorizationRequirement { }
public class ApiKeyAuthorizationHandler : AuthorizationHandler<ApiKeyRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
    {
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}