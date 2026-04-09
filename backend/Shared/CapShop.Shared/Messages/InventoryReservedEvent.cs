namespace CapShop.Shared.Messages;

public class InventoryReservedEvent
{
    public Guid CorrelationId { get; set; }
    public int OrderId { get; set; }
    public bool Success { get; set; }
    public string? FailureReason { get; set; }
}
