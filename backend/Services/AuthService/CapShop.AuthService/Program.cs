
using CapShop.AuthService.Application.Commands;
using CapShop.AuthService.Application.Interfaces;
using CapShop.AuthService.Domain.Interfaces;
using CapShop.AuthService.Infrastructure.Persistence;
using CapShop.AuthService.Infrastructure.Repositories;
using CapShop.AuthService.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ✅ Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthCommandHandler, AuthCommandHandler>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                               Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();


// Auto Migration + Admin Seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.Migrate();

    var adminEmail    = app.Configuration["Admin:Email"];
    var adminPassword = app.Configuration["Admin:Password"];
    var adminFirst    = app.Configuration["Admin:FirstName"] ?? "Admin";
    var adminLast     = app.Configuration["Admin:LastName"]  ?? "User";

    if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword)
        && !db.Users.Any(u => u.Email == adminEmail))
    {
        db.Users.Add(new CapShop.AuthService.Domain.Entities.User
        {
            Id        = Guid.NewGuid(),
            Email     = adminEmail,
            Password  = BCrypt.Net.BCrypt.HashPassword(adminPassword),
            FirstName = adminFirst,
            LastName  = adminLast,
            Role      = "Admin",
            CreatedAt = DateTime.UtcNow
        });
        db.SaveChanges();
        Console.WriteLine("✅ Admin user seeded.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();        // ✅
    app.UseSwaggerUI();      // ✅
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
