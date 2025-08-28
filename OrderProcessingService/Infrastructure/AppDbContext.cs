using Microsoft.EntityFrameworkCore;
using OrderProcessingService.Domain.Entities;

namespace OrderProcessingService.Infrastructure;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).IsRequired().HasMaxLength(200);
            e.Property(p => p.RowVersion).IsRowVersion();
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.CustomerName).IsRequired().HasMaxLength(200);
            e.HasMany(o => o.Items).WithOne().HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(e =>
        {
            e.HasKey(i => i.Id);
            e.Property(i => i.Quantity).IsRequired();
        });

        // seed a couple of products
        var p1 = new Product { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Keyboard", StockQuantity = 10 };
        var p2 = new Product { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "Mouse", StockQuantity = 20 };
        modelBuilder.Entity<Product>().HasData(p1, p2);
    }
}