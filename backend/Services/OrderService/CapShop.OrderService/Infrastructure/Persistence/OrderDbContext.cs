using CapShop.OrderService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CapShop.OrderService.Infrastructure.Persistence;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options) { }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<OrderItem>()
            .Property(o => o.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<OrderItem>()
            .Ignore(o => o.Total);

        modelBuilder.Entity<OrderItem>()
            .HasOne<Order>()
            .WithMany(o => o.Items)
            .HasForeignKey(o => o.OrderId);
    }
}