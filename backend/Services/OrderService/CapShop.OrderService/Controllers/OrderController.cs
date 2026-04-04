using CapShop.OrderService.Infrastructure.Persistence;
using CapShop.OrderService.Domain.Entities;
using CapShop.OrderService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CapShop.OrderService.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly OrderDbContext _db;
    private readonly IRabbitMQService _rabbitMQ;  // ✅ Sirf RabbitMQ

    public OrderController(OrderDbContext db, IRabbitMQService rabbitMQ)
    {
        _db = db;
        _rabbitMQ = rabbitMQ;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private string GetUserEmail() =>
        User.FindFirstValue(ClaimTypes.Email) ?? "";

    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = GetUserId();
        var orders = await _db.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetUserId();
        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
            return BadRequest(new { message = "Order mein koi item nahi hai" });

        var userEmail = GetUserEmail();
        var userName = User.FindFirstValue(ClaimTypes.GivenName) ?? "Customer";

        var order = new Order
        {
            UserId = GetUserId(),
            UserEmail = userEmail,
            ShippingAddress = dto.ShippingAddress,
            TotalAmount = dto.Items.Sum(i => i.Price * i.Quantity),
            Items = dto.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList()
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        // ✅ RabbitMQ queue mein message bhejo
        try
        {
            _rabbitMQ.PublishOrderPlaced(
                order.Id,
                userEmail,
                userName,
                order.TotalAmount,
                order.ShippingAddress
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RabbitMQ publish failed: {ex.Message}");
        }

        return Ok(new { message = "Order placed successfully", orderId = order.Id });
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound();

        order.Status = dto.Status;
        if (dto.PaymentStatus != null)
            order.PaymentStatus = dto.PaymentStatus;

        await _db.SaveChangesAsync();

        // ✅ RabbitMQ queue mein status change bhejo
        try
        {
            _rabbitMQ.PublishOrderStatusChanged(
                order.Id,
                order.UserEmail,
                order.Status,
                order.PaymentStatus
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RabbitMQ publish failed: {ex.Message}");
        }

        return Ok(new { message = "Status updated", order });
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _db.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
        return Ok(orders);
    }
}

public class PlaceOrderDto
{
    public string ShippingAddress { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class UpdateStatusDto
{
    public string Status { get; set; } = string.Empty;
    public string? PaymentStatus { get; set; }
}