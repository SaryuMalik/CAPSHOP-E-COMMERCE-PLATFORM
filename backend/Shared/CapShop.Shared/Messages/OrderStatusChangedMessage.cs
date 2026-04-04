namespace CapShop.Shared.Messages;

public class OrderStatusChangedMessage
{
    public int OrderId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
}
