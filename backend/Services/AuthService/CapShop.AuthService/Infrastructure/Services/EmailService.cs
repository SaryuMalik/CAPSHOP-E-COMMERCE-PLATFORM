using CapShop.AuthService.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace CapShop.AuthService.Infrastructure.Services;

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
    public async Task SendOtpEmailAsync(string toEmail, string otp, string firstName)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config["Email:From"]));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = "CapShop - Password Reset OTP";

        email.Body = new TextPart("html")
        {
            Text = $@"
            <div style='font-family:Inter,sans-serif; max-width:500px; margin:0 auto;'>
                <div style='background:#0071e3; padding:24px; border-radius:12px 12px 0 0;'>
                    <h2 style='color:white; margin:0;'>⚡ CapShop</h2>
                </div>
                <div style='background:#f5f5f7; padding:32px; border-radius:0 0 12px 12px;'>
                    <h3 style='color:#1d1d1f;'>Hi {firstName}!</h3>
                    <p style='color:#6e6e73;'>Your password reset OTP is:</p>
                    <div style='background:white; border:2px solid #0071e3; border-radius:10px; padding:20px; text-align:center; margin:20px 0;'>
                        <span style='font-size:36px; font-weight:900; color:#0071e3; letter-spacing:8px;'>{otp}</span>
                    </div>
                    <p style='color:#6e6e73; font-size:13px;'>This OTP is valid for <strong>10 minutes</strong>.</p>
                    <p style='color:#6e6e73; font-size:13px;'>If you didn't request this, ignore this email.</p>
                </div>
            </div>"
        };
        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _config["Email:Host"],
            int.Parse(_config["Email:Port"]!),
            SecureSocketOptions.StartTls
        );
        await smtp.AuthenticateAsync(_config["Email:From"], _config["Email:Password"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}