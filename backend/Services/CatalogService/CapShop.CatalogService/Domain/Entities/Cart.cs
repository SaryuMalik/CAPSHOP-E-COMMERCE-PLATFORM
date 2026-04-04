namespace CapShop.CatalogService.Domain.Entities;

public class Cart
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}