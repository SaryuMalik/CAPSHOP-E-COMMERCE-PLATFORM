using CapShop.CatalogService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CapShop.CatalogService.Controllers;

[ApiController]
[Route("api/categories")]
[AllowAnonymous]
public class CategoryController : ControllerBase
{
    private readonly CatalogDbContext _db;
    public CategoryController(CatalogDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Categories.ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null) return NotFound();
        return Ok(category);
    }
}