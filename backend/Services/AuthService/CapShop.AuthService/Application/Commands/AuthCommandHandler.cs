using CapShop.AuthService.Application.DTOs;
using CapShop.AuthService.Application.Interfaces;
using CapShop.AuthService.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CapShop.AuthService.Application.Commands;

public class AuthCommandHandler : IAuthCommandHandler
{
    private readonly IUserRepository _userRepo;
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;  // ✅ Added

    public AuthCommandHandler(
        IUserRepository userRepo,
        IConfiguration config,
        IEmailService emailService)  // ✅ Added
    {
        _userRepo = userRepo;
        _config = config;
        _emailService = emailService;
    }

    public async Task<AuthResult> RegisterAsync(RegisterDto dto)
    {
        if (await _userRepo.EmailExistsAsync(dto.Email))
            return new AuthResult { Success = false, Message = "Email already registered" };

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new Domain.Entities.User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            Password = hashedPassword,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Role = "Customer",
            CreatedAt = DateTime.UtcNow
        };

        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();

        // ✅ Welcome email
        try
        {
            await _emailService.SendEmailAsync(
                user.Email,
                "Welcome to CapShop! 🎉",
                $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #6c63ff;'>Welcome to CapShop, {user.FirstName}! 🛍️</h2>
                    <p>Your account has been created successfully.</p>
                    <a href='http://localhost:4200/products'
                       style='background:#6c63ff; color:white; padding:12px 24px;
                              text-decoration:none; border-radius:8px; display:inline-block;'>
                       Start Shopping →
                    </a>
                    <p style='color:#888; margin-top:20px; font-size:12px;'>
                        If you did not create this account, please ignore this email.
                    </p>
                </div>"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email send failed: {ex.Message}");
        }

        return new AuthResult { Success = true, Message = "Registered successfully" };
    }

    public async Task<AuthResult> LoginAsync(LoginDto dto)
    {
        var user = await _userRepo.GetByEmailAsync(dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            return new AuthResult { Success = false, Message = "Invalid email or password" };

        var token = GenerateJwtToken(user);

        return new AuthResult
        {
            Success = true,
            Token = token,
            User = new { user.Id, user.Email, user.FirstName, user.LastName, user.Role }
        };
    }

    private string GenerateJwtToken(Domain.Entities.User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email,          user.Email),
            new Claim(ClaimTypes.GivenName,      user.FirstName),
            new Claim(ClaimTypes.Surname,        user.LastName),
            new Claim(ClaimTypes.Role,           user.Role ?? "Customer")
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                     Convert.ToDouble(_config["Jwt:ExpiryMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}