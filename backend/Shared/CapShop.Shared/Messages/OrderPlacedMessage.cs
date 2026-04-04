namespace CapShop.Shared.Messages;

public class OrderPlacedMessage
{
    public int OrderId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
}