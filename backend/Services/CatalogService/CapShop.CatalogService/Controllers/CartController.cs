using CapShop.CatalogService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CapShop.CatalogService.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly CatalogDbContext _db;
    public CartController(CatalogDbContext db) => _db = db;

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetUserId();
        var cart = await _db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null) return Ok(new { items = new List<object>(), total = 0 });
        return Ok(cart);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] CartItemDto dto)
    {
        var userId = GetUserId();
        var product = await _db.Products.FindAsync(dto.ProductId);
        if (product == null) return NotFound(new { message = "Product not found" });
        if (product.Stock < dto.Quantity)
            return BadRequest(new { message = "Insufficient stock" });

        var cart = await _db.Carts.Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Domain.Entities.Cart { UserId = userId };
            _db.Carts.Add(cart);
        }

        var existing = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
        if (existing != null)
            existing.Quantity += dto.Quantity;
        else
            cart.Items.Add(new Domain.Entities.CartItem
            {
                ProductId = dto.ProductId,
                ProductName = product.Name,
                ProductPrice = product.Price,
                Quantity = dto.Quantity
            });

        await _db.SaveChangesAsync();
        return Ok(new { message = "Added to cart" });
    }

    [HttpDelete("remove/{productId}")]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        var userId = GetUserId();
        var cart = await _db.Carts.Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart == null) return NotFound();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null) cart.Items.Remove(item);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Removed from cart" });
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        var userId = GetUserId();
        var cart = await _db.Carts.Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        if (cart != null) { cart.Items.Clear(); await _db.SaveChangesAsync(); }
        return Ok(new { message = "Cart cleared" });
    }
}

public class CartItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}