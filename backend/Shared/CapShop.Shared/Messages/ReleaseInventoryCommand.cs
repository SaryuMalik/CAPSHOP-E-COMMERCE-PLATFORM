namespace CapShop.Shared.Messages;

public class ReleaseInventoryCommand
{
    public Guid CorrelationId { get; set; }
    public int OrderId { get; set; }
    public List<OrderItemMessage> Items { get; set; } = new();
}
