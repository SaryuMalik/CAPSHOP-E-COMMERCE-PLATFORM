namespace CapShop.AuthService.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Reset Password
    public string? ResetOtp { get; set; }
    public DateTime? ResetOtpExpiry { get; set; }
}