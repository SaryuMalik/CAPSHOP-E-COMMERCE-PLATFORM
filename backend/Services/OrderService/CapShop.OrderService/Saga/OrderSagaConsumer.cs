using CapShop.OrderService.Application.Interfaces;
using CapShop.OrderService.Domain.Interfaces;
using CapShop.Shared.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CapShop.OrderService.Saga;

public class OrderSagaConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;
    private IConnection? _connection;
    private IChannel? _channel;

    public OrderSagaConsumer(IServiceScopeFactory scopeFactory, IConfiguration config)
    {
        _scopeFactory = scopeFactory;
        _config = config;
    }

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
                await _channel.QueueDeclareAsync("inventory-reserved", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
                await _channel.QueueDeclareAsync("payment-completed", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
                await _channel.QueueDeclareAsync("payment-requested", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
                await _channel.QueueDeclareAsync("release-inventory", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
                Console.WriteLine("✅ OrderSagaConsumer connected");
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ OrderSagaConsumer attempt {attempt}/10 failed: {ex.Message}");
                if (attempt == 10) { Console.WriteLine("❌ OrderSagaConsumer: giving up."); return; }
                await Task.Delay(5000, cancellationToken);
            }
        }
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel == null) return;

        var inventoryConsumer = new AsyncEventingBasicConsumer(_channel);
        inventoryConsumer.ReceivedAsync += async (_, ea) =>
        {
            var evt = Deserialize<InventoryReservedEvent>(ea.Body);
            if (evt != null) await HandleInventoryReserved(evt);
            await _channel!.BasicAckAsync(ea.DeliveryTag, false);
        };

        var paymentConsumer = new AsyncEventingBasicConsumer(_channel!);
        paymentConsumer.ReceivedAsync += async (_, ea) =>
        {
            var evt = Deserialize<PaymentCompletedEvent>(ea.Body);
            if (evt != null) await HandlePaymentCompleted(evt);
            await _channel!.BasicAckAsync(ea.DeliveryTag, false);
        };

        await _channel!.BasicConsumeAsync("inventory-reserved", autoAck: false, consumer: inventoryConsumer);
        await _channel!.BasicConsumeAsync("payment-completed", autoAck: false, consumer: paymentConsumer);

        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);
    }

    private async Task HandleInventoryReserved(InventoryReservedEvent evt)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();

        var order = await repo.GetByCorrelationIdAsync(evt.CorrelationId);
        if (order == null) return;

        if (!evt.Success)
        {
            order.Status = "Cancelled"; order.SagaState = "InventoryFailed";
            await repo.SaveChangesAsync();
            publisher.PublishOrderStatusChanged(new OrderStatusChangedMessage
            {
                CorrelationId = evt.CorrelationId, OrderId = order.Id,
                UserEmail = order.UserEmail, Status = "Cancelled", PaymentStatus = order.PaymentStatus
            });
            return;
        }

        order.SagaState = "InventoryReserved";
        await repo.SaveChangesAsync();
        publisher.PublishPaymentRequested(new PaymentRequestedEvent
        {
            CorrelationId = evt.CorrelationId, OrderId = order.Id,
            UserEmail = order.UserEmail, TotalAmount = order.TotalAmount
        });
    }

    private async Task HandlePaymentCompleted(PaymentCompletedEvent evt)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();

        var order = await repo.GetByCorrelationIdAsync(evt.CorrelationId);
        if (order == null) return;

        if (!evt.Success)
        {
            order.Status = "Cancelled"; order.PaymentStatus = "Failed"; order.SagaState = "PaymentFailed";
            await repo.SaveChangesAsync();
            publisher.PublishReleaseInventory(new ReleaseInventoryCommand
            {
                CorrelationId = evt.CorrelationId, OrderId = order.Id,
                Items = order.Items.Select(i => new OrderItemMessage { ProductId = i.ProductId, Quantity = i.Quantity }).ToList()
            });
            publisher.PublishOrderStatusChanged(new OrderStatusChangedMessage
            {
                CorrelationId = evt.CorrelationId, OrderId = order.Id,
                UserEmail = order.UserEmail, Status = "Cancelled", PaymentStatus = "Failed"
            });
            return;
        }

        order.Status = "Confirmed"; order.PaymentStatus = "Paid"; order.SagaState = "Completed";
        await repo.SaveChangesAsync();
        publisher.PublishOrderStatusChanged(new OrderStatusChangedMessage
        {
            CorrelationId = evt.CorrelationId, OrderId = order.Id,
            UserEmail = order.UserEmail, Status = "Confirmed", PaymentStatus = "Paid"
        });
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
