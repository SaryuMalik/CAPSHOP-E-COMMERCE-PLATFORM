namespace CapShop.OrderService.Services;

public interface IRabbitMQService
{
    void PublishOrderPlaced(int orderId, string userEmail, string userName, decimal totalAmount, string shippingAddress);
    void PublishOrderStatusChanged(int orderId, string userEmail, string status, string paymentStatus);
}