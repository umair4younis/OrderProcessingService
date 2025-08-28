namespace OrderProcessingService.Domain.Entities;
public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}