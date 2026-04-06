using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CapShop.OrderService.Services;

public class RabbitMQService : IRabbitMQService, IDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;
    private bool _isConnected = false;

    public RabbitMQService(IConfiguration configuration)
    {
        try
        {
            var host = configuration["RabbitMQ:Host"] ?? "localhost";
            var port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672");
            var username = configuration["RabbitMQ:Username"] ?? "guest";
            var password = configuration["RabbitMQ:Password"] ?? "guest";

            var factory = new ConnectionFactory
            {
                HostName = host,
                Port = port,
                UserName = username,
                Password = password
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

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

            _isConnected = true;
            Console.WriteLine($"✅ RabbitMQ connected to {host}:{port}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ RabbitMQ connection failed: {ex.Message}. Orders will work without messaging.");
            _isConnected = false;
        }
    }

    public void PublishOrderPlaced(int orderId, string userEmail, string userName, decimal totalAmount, string shippingAddress)
    {
        if (!_isConnected || _channel == null)
        {
            Console.WriteLine("⚠️ RabbitMQ not connected, skipping publish.");
            return;
        }

        try
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
            _channel.BasicPublishAsync(exchange: "", routingKey: "order-placed", body: body)
                    .GetAwaiter().GetResult();

            Console.WriteLine($"✅ RabbitMQ: Order #{orderId} published!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ RabbitMQ publish failed: {ex.Message}");
        }
    }

    public void PublishOrderStatusChanged(int orderId, string userEmail, string status, string paymentStatus)
    {
        if (!_isConnected || _channel == null)
        {
            Console.WriteLine("⚠️ RabbitMQ not connected, skipping publish.");
            return;
        }

        try
        {
            var message = JsonSerializer.Serialize(new
            {
                OrderId = orderId,
                UserEmail = userEmail,
                Status = status,
                PaymentStatus = paymentStatus
            });

            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublishAsync(exchange: "", routingKey: "order-status-changed", body: body)
                    .GetAwaiter().GetResult();

            Console.WriteLine($"✅ RabbitMQ: Order #{orderId} status published!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ RabbitMQ publish failed: {ex.Message}");
        }
    }

    public void Dispose()
    {
        try { _channel?.CloseAsync().GetAwaiter().GetResult(); } catch { }
        try { _connection?.CloseAsync().GetAwaiter().GetResult(); } catch { }
    }
}
