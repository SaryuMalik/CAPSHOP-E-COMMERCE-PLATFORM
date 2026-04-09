using CapShop.Shared.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CapShop.OrderService.Saga;

public class PaymentSagaConsumer : BackgroundService
{
    private readonly IConfiguration _config;
    private IConnection? _connection;
    private IChannel? _channel;

    public PaymentSagaConsumer(IConfiguration config) { _config = config; }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var host = _config["RabbitMQ:Host"] ?? "localhost";
        var port = int.TryParse(_config["RabbitMQ:Port"], out var p) ? p : 5672;
        var user = _config["RabbitMQ:Username"] ?? "guest";
        var pass = _config["RabbitMQ:Password"] ?? "guest";

        for (int attempt = 1; attempt <= 10; attempt++)
        {
            try
            {
                var factory = new ConnectionFactory { HostName = host, Port = port, UserName = user, Password = pass };
                _connection = await factory.CreateConnectionAsync(cancellationToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
                await _channel.QueueDeclareAsync("payment-requested", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
                await _channel.QueueDeclareAsync("payment-completed", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
                Console.WriteLine("✅ PaymentSagaConsumer connected");
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ PaymentSagaConsumer attempt {attempt}/10 failed: {ex.Message}");
                if (attempt == 10) { Console.WriteLine("❌ PaymentSagaConsumer: giving up."); return; }
                await Task.Delay(5000, cancellationToken);
            }
        }
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel == null) return;

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var evt = Deserialize<PaymentRequestedEvent>(ea.Body);
            if (evt != null) await HandlePaymentRequested(evt);
            await _channel!.BasicAckAsync(ea.DeliveryTag, false);
        };

        await _channel!.BasicConsumeAsync("payment-requested", autoAck: false, consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);
    }

    private async Task HandlePaymentRequested(PaymentRequestedEvent evt)
    {
        await Task.Delay(500); // simulate processing
        var success = true;
        Publish("payment-completed", new PaymentCompletedEvent
        {
            CorrelationId = evt.CorrelationId,
            OrderId = evt.OrderId,
            Success = success,
            FailureReason = success ? null : "Payment gateway declined"
        });
    }

    private void Publish<T>(string queue, T message)
    {
        try
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            _channel!.BasicPublishAsync(exchange: "", routingKey: queue, body: body).GetAwaiter().GetResult();
        }
        catch (Exception ex) { Console.WriteLine($"⚠️ Failed to publish to [{queue}]: {ex.Message}"); }
    }

    private static T? Deserialize<T>(ReadOnlyMemory<byte> body)
    {
        try { return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(body.ToArray())); }
        catch { return default; }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null) await _channel.CloseAsync();
        if (_connection != null) await _connection.CloseAsync();
        await base.StopAsync(cancellationToken);
    }
}
