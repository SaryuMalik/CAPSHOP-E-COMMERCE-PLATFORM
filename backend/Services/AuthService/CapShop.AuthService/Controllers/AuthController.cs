using CapShop.AuthService.Application.Commands;
using CapShop.AuthService.Application.DTOs;
using CapShop.AuthService.Application.Interfaces;
using CapShop.AuthService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CapShop.AuthService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthCommandHandler _handler;
    private readonly IEmailService _emailService;
    private readonly AuthDbContext _db;

    public AuthController(
        IAuthCommandHandler handler,
        IEmailService emailService,
        AuthDbContext db)
    {
        _handler = handler;
        _emailService = emailService;
        _db = db;
    }


    // POST api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _handler.RegisterAsync(dto);
        if (!result.Success) return BadRequest(new { message = result.Message });

        return Ok(new { message = "Registration successful" });
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _handler.LoginAsync(dto);
        if (!result.Success) return Unauthorized(new { message = result.Message });

        return Ok(new { token = result.Token, user = result.User });
    }

    // POST api/auth/forgot-password
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
            return Ok(new { message = "If email exists, OTP has been sent" });

        // OTP generate karo
        var otp = new Random().Next(100000, 999999).ToString();
        user.ResetOtp = otp;
        user.ResetOtpExpiry = DateTime.UtcNow.AddMinutes(10);
        await _db.SaveChangesAsync();

        // Email bhejo
        await _emailService.SendOtpEmailAsync(user.Email, otp, user.FirstName);

        return Ok(new { message = "OTP sent to your email!" });
    }

    // POST api/auth/reset-password
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || user.ResetOtp != dto.Otp)
            return BadRequest(new { message = "Invalid OTP!" });

        if (user.ResetOtpExpiry < DateTime.UtcNow)
            return BadRequest(new { message = "OTP expired! Request a new one." });

        user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.ResetOtp = null;
        user.ResetOtpExpiry = null;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Password reset successfully!" });
    }
}