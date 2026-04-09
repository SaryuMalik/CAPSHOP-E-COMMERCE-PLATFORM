using CapShop.OrderService.Application.Interfaces;
using CapShop.Shared.Messages;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CapShop.OrderService.Infrastructure.Messaging;

public class MessagePublisher : IMessagePublisher, IDisposable
{
    private readonly IConfiguration _config;
    private IConnection? _connection;
    private IChannel? _channel;
    private bool _isConnected;

    private static readonly string[] Queues =
    [
        "order-placed", "order-status-changed",
        "payment-requested", "release-inventory"
    ];

    public MessagePublisher(IConfiguration config) { _config = config; }

    private void EnsureConnected()
    {
        if (_isConnected) return;
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _config["RabbitMQ:Host"] ?? "localhost",
                Port = int.Parse(_config["RabbitMQ:Port"] ?? "5672"),
                UserName = _config["RabbitMQ:Username"] ?? "guest",
                Password = _config["RabbitMQ:Password"] ?? "guest"
            };
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
            foreach (var q in Queues)
                _channel.QueueDeclareAsync(q, durable: true, exclusive: false, autoDelete: false)
                        .GetAwaiter().GetResult();
            _isConnected = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ MessagePublisher: RabbitMQ unavailable — {ex.Message}");
        }
    }

    public void PublishOrderPlaced(OrderPlacedMessage message) => Publish("order-placed", message);
    public void PublishOrderStatusChanged(OrderStatusChangedMessage message) => Publish("order-status-changed", message);
    public void PublishPaymentRequested(PaymentRequestedEvent message) => Publish("payment-requested", message);
    public void PublishReleaseInventory(ReleaseInventoryCommand message) => Publish("release-inventory", message);

    public void Publish(string queue, object message)
    {
        EnsureConnected();
        if (!_isConnected || _channel == null) return;
        try
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            _channel.BasicPublishAsync(exchange: "", routingKey: queue, body: body)
                    .GetAwaiter().GetResult();
        }
        catch (Exception ex) { Console.WriteLine($"⚠️ Publish to [{queue}] failed: {ex.Message}"); }
    }

    public void Dispose()
    {
        try { _channel?.CloseAsync().GetAwaiter().GetResult(); } catch { }
        try { _connection?.CloseAsync().GetAwaiter().GetResult(); } catch { }
    }
}
