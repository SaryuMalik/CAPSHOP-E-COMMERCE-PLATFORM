using CapShop.CatalogService.Domain.Interfaces;
using CapShop.Shared.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CapShop.CatalogService.Saga;

public class InventorySagaConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;
    private IConnection? _connection;
    private IChannel? _channel;

    public InventorySagaConsumer(IServiceScopeFactory scopeFactory, IConfiguration config)
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
                await _channel.QueueDeclareAsync("order-placed", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
                await _channel.QueueDeclareAsync("inventory-reserved", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
                await _channel.QueueDeclareAsync("release-inventory", durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
                Console.WriteLine("✅ InventorySagaConsumer connected to RabbitMQ");
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ InventorySagaConsumer attempt {attempt}/10 failed: {ex.Message}");
                if (attempt == 10)
                {
                    Console.WriteLine("❌ InventorySagaConsumer: giving up on RabbitMQ, saga disabled.");
                    return;
                }
                await Task.Delay(5000, cancellationToken);
            }
        }

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel == null) return;

        var orderPlacedConsumer = new AsyncEventingBasicConsumer(_channel);
        orderPlacedConsumer.ReceivedAsync += async (_, ea) =>
        {
            var msg = Deserialize<OrderPlacedMessage>(ea.Body);
            if (msg != null) await HandleOrderPlaced(msg);
            await _channel!.BasicAckAsync(ea.DeliveryTag, false);
        };

        var releaseConsumer = new AsyncEventingBasicConsumer(_channel!);
        releaseConsumer.ReceivedAsync += async (_, ea) =>
        {
            var cmd = Deserialize<ReleaseInventoryCommand>(ea.Body);
            if (cmd != null) await HandleReleaseInventory(cmd);
            await _channel!.BasicAckAsync(ea.DeliveryTag, false);
        };

        await _channel!.BasicConsumeAsync("order-placed", autoAck: false, consumer: orderPlacedConsumer);
        await _channel!.BasicConsumeAsync("release-inventory", autoAck: false, consumer: releaseConsumer);

        Console.WriteLine("👂 InventorySagaConsumer listening...");
        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);
    }

    private async Task HandleOrderPlaced(OrderPlacedMessage msg)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

        Console.WriteLine($"📦 Saga [{msg.CorrelationId}]: Reserving inventory for Order #{msg.OrderId}");

        foreach (var item in msg.Items)
        {
            var product = await repo.GetByIdAsync(item.ProductId);
            if (product == null || product.Stock < item.Quantity)
            {
                Publish("inventory-reserved", new InventoryReservedEvent
                {
                    CorrelationId = msg.CorrelationId, OrderId = msg.OrderId,
                    Success = false, FailureReason = $"Product {item.ProductId} out of stock"
                });
                Console.WriteLine($"❌ Saga [{msg.CorrelationId}]: Insufficient stock for Product #{item.ProductId}");
                return;
            }
        }

        foreach (var item in msg.Items)
        {
            var product = await repo.GetByIdAsync(item.ProductId);
            product!.Stock -= item.Quantity;
        }
        await repo.SaveChangesAsync();

        Publish("inventory-reserved", new InventoryReservedEvent
        {
            CorrelationId = msg.CorrelationId, OrderId = msg.OrderId, Success = true
        });
        Console.WriteLine($"✅ Saga [{msg.CorrelationId}]: Inventory reserved for Order #{msg.OrderId}");
    }

    private async Task HandleReleaseInventory(ReleaseInventoryCommand cmd)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

        foreach (var item in cmd.Items)
        {
            var product = await repo.GetByIdAsync(item.ProductId);
            if (product != null) product.Stock += item.Quantity;
        }
        await repo.SaveChangesAsync();
        Console.WriteLine($"✅ Saga [{cmd.CorrelationId}]: Inventory released for Order #{cmd.OrderId}");
    }

    private void Publish<T>(string queue, T message)
    {
        try
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            _channel!.BasicPublishAsync(exchange: "", routingKey: queue, body: body)
                    .GetAwaiter().GetResult();
        }
        catch (Exception ex) { Console.WriteLine($"⚠️ Publish failed: {ex.Message}"); }
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
