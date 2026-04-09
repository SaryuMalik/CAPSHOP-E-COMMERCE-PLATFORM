namespace CapShop.Shared.Messages;

public class PaymentRequestedEvent
{
    public Guid CorrelationId { get; set; }
    public int OrderId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}
