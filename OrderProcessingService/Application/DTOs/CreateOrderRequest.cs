using System.ComponentModel.DataAnnotations;

namespace OrderProcessingService.Application.DTOs;
public class CreateOrderRequest
{
    [Required, MinLength(1)]
    public string CustomerName { get; set; } = string.Empty;
    [Required, MinLength(1)]
    public List<CreateOrderItemDto> Items { get; set; } = new();
}
public class CreateOrderItemDto
{
    [Required]
    public Guid ProductId { get; set; }
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}