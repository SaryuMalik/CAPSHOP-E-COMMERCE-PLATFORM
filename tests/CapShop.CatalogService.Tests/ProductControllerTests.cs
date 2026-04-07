using CapShop.CatalogService.Controllers;
using CapShop.CatalogService.Domain.Entities;
using CapShop.CatalogService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Security.Claims;

namespace CapShop.CatalogService.Tests;

[TestFixture]
public class ProductControllerTests
{
    private CatalogDbContext _db;
    private Mock<IDistributedCache> _cacheMock;
    private ProductController _controller;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db        = new CatalogDbContext(options);
        _cacheMock = new Mock<IDistributedCache>();

        // Cache miss by default
        _cacheMock.Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((byte[]?)null);
        _cacheMock.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(),
                  It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                  .Returns(Task.CompletedTask);
        _cacheMock.Setup(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .Returns(Task.CompletedTask);

        _controller = new ProductController(_db, _cacheMock.Object);

        // Simulate admin user
        var claims    = new List<Claim> { new(ClaimTypes.Role, "Admin") };
        var identity  = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        SeedData();
    }

    private void SeedData()
    {
        _db.Categories.AddRange(
            new Category { Id = 1, Name = "Electronics", Description = "Gadgets" },
            new Category { Id = 2, Name = "Books",       Description = "Books" }
        );
        _db.Products.AddRange(
            new Product { Id = 1, Name = "iPhone 15",   Price = 999m,  Stock = 50, CategoryId = 1, CategoryName = "Electronics", IsActive = true,  CreatedAt = DateTime.UtcNow },
            new Product { Id = 2, Name = "MacBook Pro",  Price = 1999m, Stock = 20, CategoryId = 1, CategoryName = "Electronics", IsActive = true,  CreatedAt = DateTime.UtcNow },
            new Product { Id = 3, Name = "Clean Code",   Price = 39m,   Stock = 10, CategoryId = 2, CategoryName = "Books",       IsActive = true,  CreatedAt = DateTime.UtcNow },
            new Product { Id = 4, Name = "Inactive Item", Price = 10m,  Stock = 5,  CategoryId = 1, CategoryName = "Electronics", IsActive = false, CreatedAt = DateTime.UtcNow }
        );
        _db.SaveChanges();
    }

    [TearDown]
    public void TearDown() => _db.Dispose();

    // ── GetAll Tests ────────────────────────────────────────────────

    [Test]
    public async Task GetAll_ReturnsOkResult()
    {
        var result = await _controller.GetAll(1, 10);
        Assert.That(result, Is.InstanceOf<ContentResult>());
    }

    [Test]
    public async Task GetAll_ReturnsOnlyActiveProducts()
    {
        var result = await _controller.GetAll(1, 10) as ContentResult;
        Assert.That(result, Is.Not.Null);
        // Response is JSON — inactive product should not appear
        Assert.That(result!.Content, Does.Not.Contain("Inactive Item"));
    }

    [Test]
    public async Task GetAll_RespectsPageSize()
    {
        var result = await _controller.GetAll(1, 2) as ContentResult;
        Assert.That(result, Is.Not.Null);
        // With pageSize=2, only 2 products in data array
        var count = result!.Content!.Split("\"name\"").Length - 1;
        Assert.That(count, Is.LessThanOrEqualTo(2));
    }

    // ── GetById Tests ───────────────────────────────────────────────

    [Test]
    public async Task GetById_WithValidId_ReturnsProduct()
    {
        var result = await _controller.GetById(1) as ContentResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Content, Does.Contain("iPhone 15"));
    }

    [Test]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        var result = await _controller.GetById(9999);
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task GetById_WithInactiveProduct_ReturnsNotFound()
    {
        // Product 4 is inactive
        var result = await _controller.GetById(4);
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    // ── GetByCategory Tests ─────────────────────────────────────────

    [Test]
    public async Task GetByCategory_ReturnsOnlyMatchingCategory()
    {
        var result = await _controller.GetByCategory(2) as ContentResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Content, Does.Contain("Clean Code"));
        Assert.That(result.Content, Does.Not.Contain("iPhone 15"));
    }

    // ── Create Tests ────────────────────────────────────────────────

    [Test]
    public async Task Create_WithValidDto_ReturnsOk()
    {
        var dto = new ProductDto
        {
            Name        = "New Product",
            Description = "Test",
            Price       = 299m,
            Stock       = 30,
            CategoryId  = 1,
            ImageUrl    = ""
        };

        var result = await _controller.Create(dto) as OkObjectResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task Create_SavesProductToDatabase()
    {
        var dto = new ProductDto
        {
            Name       = "DB Test Product",
            Price      = 100m,
            Stock      = 10,
            CategoryId = 2,
            ImageUrl   = ""
        };

        await _controller.Create(dto);

        var saved = await _db.Products.FirstOrDefaultAsync(p => p.Name == "DB Test Product");
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.CategoryName, Is.EqualTo("Books"));
    }

    [Test]
    public async Task Create_SetsCorrectCategoryName()
    {
        var dto = new ProductDto { Name = "Test", Price = 10m, Stock = 5, CategoryId = 1, ImageUrl = "" };

        var result = await _controller.Create(dto) as OkObjectResult;
        var product = result!.Value as Product;

        Assert.That(product!.CategoryName, Is.EqualTo("Electronics"));
    }

    // ── Update Tests ────────────────────────────────────────────────

    [Test]
    public async Task Update_WithValidId_UpdatesFields()
    {
        var dto = new ProductDto
        {
            Name       = "iPhone 15 Pro",
            Price      = 1099m,
            Stock      = 45,
            CategoryId = 1,
            ImageUrl   = ""
        };

        await _controller.Update(1, dto);

        var product = await _db.Products.FindAsync(1);
        Assert.That(product!.Name, Is.EqualTo("iPhone 15 Pro"));
        Assert.That(product.Price, Is.EqualTo(1099m));
    }

    [Test]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        var dto = new ProductDto { Name = "X", Price = 1m, Stock = 1, CategoryId = 1, ImageUrl = "" };
        var result = await _controller.Update(9999, dto);
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    // ── Delete Tests ────────────────────────────────────────────────

    [Test]
    public async Task Delete_SetsIsActiveToFalse()
    {
        // Soft delete — sets IsActive = false
        await _controller.Delete(1);

        var product = await _db.Products.FindAsync(1);
        Assert.That(product!.IsActive, Is.False);
    }

    [Test]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        var result = await _controller.Delete(9999);
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}
