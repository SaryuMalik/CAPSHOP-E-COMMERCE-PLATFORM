using CapShop.Shared.Messages;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CapShop.NotificationService;

public class NotificationWorker : BackgroundService
{
    private readonly IConfiguration _config;
    private IConnection? _connection;
    private IChannel? _channel;

    public NotificationWorker(IConfiguration config)
    {
        _config = config;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _config["RabbitMQ:Host"] ?? "rabbitmq",
            Port = int.TryParse(_config["RabbitMQ:Port"], out var port) ? port : 5672,
            UserName = _config["RabbitMQ:Username"] ?? "guest",
            Password = _config["RabbitMQ:Password"] ?? "guest"
        };

        int retries = 10;
        while (retries > 0)
        {
            try
            {
                Console.WriteLine($"🔄 Connecting to RabbitMQ at {factory.HostName}...");
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.QueueDeclareAsync(
                    queue: "order-status-changed",
                    durable: true,
                    exclusive: false,
                    autoDelete: false
                );

                Console.WriteLine("✅ NotificationService connected to RabbitMQ!");
                break;
            }
            catch (Exception ex)
            {
                retries--;
                Console.WriteLine($"⚠️ RabbitMQ not ready. Retries left: {retries}. Waiting 5s... Error: {ex.Message}");
                if (retries == 0) throw;
                await Task.Delay(5000, cancellationToken);
            }
        }

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var statusChangedConsumer = new AsyncEventingBasicConsumer(_channel!);
        statusChangedConsumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = JsonSerializer.Deserialize<OrderStatusChangedMessage>(
                Encoding.UTF8.GetString(body)
            );

            if (message != null)
            {
                Console.WriteLine($"🔄 Order status changed: #{message.OrderId} → {message.Status}");
                await SendStatusUpdateEmail(message);
                await _channel!.BasicAckAsync(ea.DeliveryTag, false);
            }
        };

        await _channel!.BasicConsumeAsync(
            queue: "order-status-changed",
            autoAck: false,
            consumer: statusChangedConsumer
        );

        Console.WriteLine("👂 Listening for messages...");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task SendStatusUpdateEmail(OrderStatusChangedMessage message)
    {
        try
        {
            await SendEmailAsync(
                message.UserEmail,
                $"Order #{message.OrderId} Status Updated!",
                $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #6c63ff;'>Order Status Updated!</h2>
                    <p>Your order <strong>#{message.OrderId}</strong> status has been updated.</p>
                    <p><strong>New Status:</strong> {message.Status}</p>
                    <p><strong>Payment Status:</strong> {message.PaymentStatus}</p>
                    <a href='http://localhost:4200/orders'
                       style='background:#6c63ff; color:white; padding:12px 24px;
                              text-decoration:none; border-radius:8px; display:inline-block;'>
                       View Order →
                    </a>
                </div>"
            );
            Console.WriteLine($"✅ Status update email sent to {message.UserEmail}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Email failed: {ex.Message}");
        }
    }

    private async Task SendEmailAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config["Email:From"]));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart("html") { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _config["Email:Host"],
            int.Parse(_config["Email:Port"]!),
            SecureSocketOptions.StartTls
        );
        await smtp.AuthenticateAsync(
            _config["Email:From"],
            _config["Email:Password"]
        );
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null) await _channel.CloseAsync();
        if (_connection != null) await _connection.CloseAsync();
        await base.StopAsync(cancellationToken);
    }
}