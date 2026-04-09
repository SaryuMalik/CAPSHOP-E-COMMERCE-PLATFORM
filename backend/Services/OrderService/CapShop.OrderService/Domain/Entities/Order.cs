namespace CapShop.OrderService.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending";
    public string PaymentStatus { get; set; } = "Unpaid";
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;

    // Saga correlation
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
    public string SagaState { get; set; } = "OrderPlaced";

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}