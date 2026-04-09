using CapShop.OrderService.Application.DTOs;
using CapShop.OrderService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CapShop.OrderService.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out var id) ? id : null;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();
        return Ok(await _orderService.GetMyOrdersAsync(userId.Value));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();
        var order = await _orderService.GetByIdAsync(id, userId.Value);
        return order == null ? NotFound() : Ok(order);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllOrders()
    {
        return Ok(await _orderService.GetAllAsync());
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        if (dto.Items == null || !dto.Items.Any())
            return BadRequest(new { message = "Order mein koi item nahi hai" });

        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? "";
        var userName = User.FindFirstValue(ClaimTypes.GivenName) ?? "Customer";

        var orderId = await _orderService.PlaceOrderAsync(dto, userId.Value, userEmail, userName);
        return Ok(new { message = "Order placed successfully", orderId });
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        try
        {
            await _orderService.UpdateStatusAsync(id, dto);
            return Ok(new { message = "Status updated" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
