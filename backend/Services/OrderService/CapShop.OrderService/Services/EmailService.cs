using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace CapShop.OrderService.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
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
}