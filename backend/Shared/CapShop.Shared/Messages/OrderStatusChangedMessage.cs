namespace CapShop.Shared.Messages;

public class OrderStatusChangedMessage
{
    public Guid CorrelationId { get; set; }
    public int OrderId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
}
