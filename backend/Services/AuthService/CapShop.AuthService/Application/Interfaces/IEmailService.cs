namespace CapShop.AuthService.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendOtpEmailAsync(string email, string otp, string name);
}