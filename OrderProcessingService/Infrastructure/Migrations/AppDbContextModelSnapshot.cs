using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace OrderProcessingService.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("OrderProcessing.Domain.Entities.Product", b =>
            {
                b.Property<Guid>("Id").HasColumnType("TEXT");
                b.Property<string>("Name").IsRequired().HasMaxLength(200).HasColumnType("TEXT");
                b.Property<int>("StockQuantity").HasColumnType("INTEGER");
                b.Property<byte[]>("RowVersion").IsConcurrencyToken().ValueGeneratedOnAddOrUpdate().HasColumnType("BLOB");
                b.HasKey("Id");
                b.ToTable("Products");
                b.HasData(
                    new { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Keyboard", StockQuantity = 10 },
                    new { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "Mouse", StockQuantity = 20 }
                );
            });

            modelBuilder.Entity("OrderProcessing.Domain.Entities.Order", b =>
            {
                b.Property<Guid>("Id").HasColumnType("TEXT");
                b.Property<string>("CustomerName").IsRequired().HasMaxLength(200).HasColumnType("TEXT");
                b.Property<DateTime>("CreatedAtUtc").HasColumnType("TEXT");
                b.Property<int>("Status").HasColumnType("INTEGER");
                b.HasKey("Id");
                b.ToTable("Orders");
            });

            modelBuilder.Entity("OrderProcessing.Domain.Entities.OrderItem", b =>
            {
                b.Property<Guid>("Id").HasColumnType("TEXT");
                b.Property<Guid>("OrderId").HasColumnType("TEXT");
                b.Property<Guid>("ProductId").HasColumnType("TEXT");
                b.Property<int>("Quantity").HasColumnType("INTEGER");
                b.HasKey("Id");
                b.HasIndex("OrderId");
                b.ToTable("OrderItems");
            });
#pragma warning restore 612, 618
        }
    }
}