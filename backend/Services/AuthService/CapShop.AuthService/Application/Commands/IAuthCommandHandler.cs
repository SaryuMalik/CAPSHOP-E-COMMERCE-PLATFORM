using CapShop.AuthService.Application.DTOs;

namespace CapShop.AuthService.Application.Commands;

public interface IAuthCommandHandler
{
    Task<AuthResult> RegisterAsync(RegisterDto dto);
    Task<AuthResult> LoginAsync(LoginDto dto);  // ✅ LoginDto hona chahiye, RegisterDto nahi
}

public class AuthResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? Token { get; set; }
    public object? User { get; set; }
}