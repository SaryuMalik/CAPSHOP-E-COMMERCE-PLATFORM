using CapShop.OrderService.Controllers;
using CapShop.OrderService.Domain.Entities;
using CapShop.OrderService.Infrastructure.Persistence;
using CapShop.OrderService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace CapShop.OrderService.Tests;

[TestFixture]
public class OrderControllerTests
{
    private OrderDbContext _db;
    private Mock<IRabbitMQService> _rabbitMock;
    private OrderController _controller;
    private Guid _userId;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _db          = new OrderDbContext(options);
        _rabbitMock  = new Mock<IRabbitMQService>();
        _controller  = new OrderController(_db, _rabbitMock.Object);
        _userId      = Guid.NewGuid();

        // Simulate authenticated user
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, _userId.ToString()),
            new(ClaimTypes.Email,          "test@test.com"),
            new(ClaimTypes.GivenName,      "Test")
        };
        var identity  = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [TearDown]
    public void TearDown() => _db.Dispose();

    // ── PlaceOrder Tests ────────────────────────────────────────────

    [Test]
    public async Task PlaceOrder_WithValidData_ReturnsOkWithOrderId()
    {
        // Arrange
        var dto = new PlaceOrderDto
        {
            ShippingAddress = "123 Test Street, Mumbai",
            Items = new List<OrderItemDto>
            {
                new() { ProductId = 1, ProductName = "iPhone 15", Price = 999.99m, Quantity = 1 }
            }
        };

        // Act
        var result = await _controller.PlaceOrder(dto) as OkObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task PlaceOrder_SavesOrderToDatabase()
    {
        // Arrange
        var dto = new PlaceOrderDto
        {
            ShippingAddress = "456 Test Ave, Delhi",
            Items = new List<OrderItemDto>
            {
                new() { ProductId = 2, ProductName = "MacBook Pro", Price = 1999.99m, Quantity = 1 }
            }
        };

        // Act
        await _controller.PlaceOrder(dto);

        // Assert
        var order = await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync();
        Assert.That(order, Is.Not.Null);
        Assert.That(order!.UserId, Is.EqualTo(_userId));
        Assert.That(order.ShippingAddress, Is.EqualTo("456 Test Ave, Delhi"));
        Assert.That(order.Items.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task PlaceOrder_CalculatesTotalCorrectly()
    {
        // Arrange
        var dto = new PlaceOrderDto
        {
            ShippingAddress = "Test Address",
            Items = new List<OrderItemDto>
            {
                new() { ProductId = 1, ProductName = "Product A", Price = 100m, Quantity = 2 },
                new() { ProductId = 2, ProductName = "Product B", Price = 50m,  Quantity = 3 }
            }
        };

        // Act
        await _controller.PlaceOrder(dto);

        // Assert — 100*2 + 50*3 = 350
        var order = await _db.Orders.FirstOrDefaultAsync();
        Assert.That(order!.TotalAmount, Is.EqualTo(350m));
    }

    [Test]
    public async Task PlaceOrder_WithEmptyItems_ReturnsBadRequest()
    {
        // Arrange
        var dto = new PlaceOrderDto
        {
            ShippingAddress = "Test Address",
            Items = new List<OrderItemDto>()
        };

        // Act
        var result = await _controller.PlaceOrder(dto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task PlaceOrder_SetsDefaultStatusToPending()
    {
        // Arrange
        var dto = new PlaceOrderDto
        {
            ShippingAddress = "Test",
            Items = new List<OrderItemDto>
            {
                new() { ProductId = 1, ProductName = "Test", Price = 10m, Quantity = 1 }
            }
        };

        // Act
        await _controller.PlaceOrder(dto);

        // Assert
        var order = await _db.Orders.FirstOrDefaultAsync();
        Assert.That(order!.Status, Is.EqualTo("Pending"));
        Assert.That(order.PaymentStatus, Is.EqualTo("Unpaid"));
    }

    // ── GetMyOrders Tests ───────────────────────────────────────────

    [Test]
    public async Task GetMyOrders_ReturnsOnlyCurrentUserOrders()
    {
        // Arrange — seed orders for two different users
        var otherUserId = Guid.NewGuid();

        _db.Orders.AddRange(
            new Order { UserId = _userId,      UserEmail = "test@test.com",  ShippingAddress = "A", TotalAmount = 100 },
            new Order { UserId = _userId,      UserEmail = "test@test.com",  ShippingAddress = "B", TotalAmount = 200 },
            new Order { UserId = otherUserId,  UserEmail = "other@test.com", ShippingAddress = "C", TotalAmount = 300 }
        );
        await _db.SaveChangesAsync();

        // Act
        var result = await _controller.GetMyOrders() as OkObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        var orders = result!.Value as IEnumerable<object>;
        Assert.That(orders!.Count(), Is.EqualTo(2));
    }

    // ── UpdateStatus Tests ──────────────────────────────────────────

    [Test]
    public async Task UpdateStatus_ChangesOrderStatus()
    {
        // Arrange
        var order = new Order
        {
            UserId          = _userId,
            UserEmail       = "test@test.com",
            ShippingAddress = "Test",
            TotalAmount     = 100,
            Status          = "Pending"
        };
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        var dto = new UpdateStatusDto { Status = "Delivered", PaymentStatus = "Paid" };

        // Act
        await _controller.UpdateStatus(order.Id, dto);

        // Assert
        var updated = await _db.Orders.FindAsync(order.Id);
        Assert.That(updated!.Status, Is.EqualTo("Delivered"));
        Assert.That(updated.PaymentStatus, Is.EqualTo("Paid"));
    }

    [Test]
    public async Task UpdateStatus_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.UpdateStatus(9999, new UpdateStatusDto { Status = "Delivered" });

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}
