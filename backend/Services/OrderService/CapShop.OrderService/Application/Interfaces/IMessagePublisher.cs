using CapShop.Shared.Messages;

namespace CapShop.OrderService.Application.Interfaces;

public interface IMessagePublisher
{
    void Publish(string queue, object message);
    void PublishOrderPlaced(OrderPlacedMessage message);
    void PublishOrderStatusChanged(OrderStatusChangedMessage message);
    void PublishPaymentRequested(PaymentRequestedEvent message);
    void PublishReleaseInventory(ReleaseInventoryCommand message);
}
