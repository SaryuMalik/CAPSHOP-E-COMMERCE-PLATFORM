using CapShop.CatalogService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CapShop.CatalogService.Controllers;

[ApiController]
[Route("api/products")]
[AllowAnonymous]
public class ProductController : ControllerBase
{
    private readonly CatalogDbContext _db;
    private readonly IDistributedCache _cache;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase  // ✅ CamelCase
    };

    public ProductController(CatalogDbContext db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var cacheKey = $"products_page{page}_size{pageSize}";

        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            Console.WriteLine($"✅ Redis Cache HIT: {cacheKey}");
            return Content(cached, "application/json");
        }

        Console.WriteLine($"❌ Redis Cache MISS: {cacheKey}");

        var query = _db.Products.Where(p => p.IsActive);
        var total = await query.CountAsync();
        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new { data = products, total, page, pageSize, totalPages = (int)Math.Ceiling((double)total / pageSize) };
        var json = JsonSerializer.Serialize(result, _jsonOptions);  // ✅ CamelCase

        await _cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        return Content(json, "application/json");
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var cacheKey = $"product_{id}";

        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            Console.WriteLine($"✅ Redis Cache HIT: {cacheKey}");
            return Content(cached, "application/json");
        }

        var product = await _db.Products.FindAsync(id);
        if (product == null || !product.IsActive) return NotFound();

        var json = JsonSerializer.Serialize(product, _jsonOptions);  // ✅ CamelCase

        await _cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });

        return Content(json, "application/json");
    }

    [HttpGet("category/{categoryId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var cacheKey = $"products_category_{categoryId}";

        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
        {
            Console.WriteLine($"✅ Redis Cache HIT: {cacheKey}");
            return Content(cached, "application/json");
        }

        var products = await _db.Products
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .ToListAsync();

        var json = JsonSerializer.Serialize(products, _jsonOptions);  // ✅ CamelCase

        await _cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        return Content(json, "application/json");
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ProductDto dto)
    {
        var category = await _db.Categories.FindAsync(dto.CategoryId);
        var product = new Domain.Entities.Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            ImageUrl = dto.ImageUrl,
            CategoryId = dto.CategoryId,
            CategoryName = category?.Name ?? ""
        };
        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        // ✅ Cache clear karo
        await _cache.RemoveAsync("products_page1_size12");

        return Ok(product);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] ProductDto dto)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();
        var category = await _db.Categories.FindAsync(dto.CategoryId);
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.ImageUrl = dto.ImageUrl;
        product.CategoryId = dto.CategoryId;
        product.CategoryName = category?.Name ?? "";
        await _db.SaveChangesAsync();

        // ✅ Cache clear karo
        await _cache.RemoveAsync($"product_{id}");
        await _cache.RemoveAsync("products_page1_size12");

        return Ok(product);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();
        product.IsActive = false;
        await _db.SaveChangesAsync();

        // ✅ Cache clear karo
        await _cache.RemoveAsync($"product_{id}");
        await _cache.RemoveAsync("products_page1_size12");

        return Ok(new { message = "Product deleted" });
    }
}

public class ProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int CategoryId { get; set; }
}