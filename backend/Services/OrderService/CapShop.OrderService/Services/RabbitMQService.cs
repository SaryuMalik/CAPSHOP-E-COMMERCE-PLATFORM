using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CapShop.OrderService.Services;

public class RabbitMQService : IRabbitMQService, IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMQService()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        // Queues declare karo
        _channel.QueueDeclareAsync(
            queue: "order-placed",
            durable: true,
            exclusive: false,
            autoDelete: false
        ).GetAwaiter().GetResult();

        _channel.QueueDeclareAsync(
            queue: "order-status-changed",
            durable: true,
            exclusive: false,
            autoDelete: false
        ).GetAwaiter().GetResult();
    }

    public void PublishOrderPlaced(int orderId, string userEmail, string userName, decimal totalAmount, string shippingAddress)
    {
        var message = JsonSerializer.Serialize(new
        {
            OrderId = orderId,
            UserEmail = userEmail,
            UserName = userName,
            TotalAmount = totalAmount,
            ShippingAddress = shippingAddress
        });

        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublishAsync(
            exchange: "",
            routingKey: "order-placed",
            body: body
        ).GetAwaiter().GetResult();

        Console.WriteLine($"✅ RabbitMQ: Order #{orderId} message published!");
    }

    public void PublishOrderStatusChanged(int orderId, string userEmail, string status, string paymentStatus)
    {
        var message = JsonSerializer.Serialize(new
        {
            OrderId = orderId,
            UserEmail = userEmail,
            Status = status,
            PaymentStatus = paymentStatus
        });

        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublishAsync(
            exchange: "",
            routingKey: "order-status-changed",
            body: body
        ).GetAwaiter().GetResult();

        Console.WriteLine($"✅ RabbitMQ: Order #{orderId} status change published!");
    }

    public void Dispose()
    {
        _channel?.CloseAsync().GetAwaiter().GetResult();
        _connection?.CloseAsync().GetAwaiter().GetResult();
    }
}