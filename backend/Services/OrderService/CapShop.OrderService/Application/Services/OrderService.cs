using CapShop.OrderService.Application.DTOs;
using CapShop.OrderService.Application.Interfaces;
using CapShop.OrderService.Domain.Entities;
using CapShop.OrderService.Domain.Interfaces;
using CapShop.Shared.Messages;

namespace CapShop.OrderService.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repo;
    private readonly IMessagePublisher _publisher;

    public OrderService(IOrderRepository repo, IMessagePublisher publisher)
    {
        _repo = repo;
        _publisher = publisher;
    }

    public async Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(Guid userId) =>
        (await _repo.GetByUserIdAsync(userId)).Select(Map);

    public async Task<OrderResponseDto?> GetByIdAsync(int id, Guid userId)
    {
        var order = await _repo.GetByIdAsync(id, userId);
        return order == null ? null : Map(order);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(Map);

    public async Task<int> PlaceOrderAsync(PlaceOrderDto dto, Guid userId, string userEmail, string userName)
    {
        var order = new Order
        {
            UserId = userId,
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

        await _repo.AddAsync(order);
        await _repo.SaveChangesAsync();

        _publisher.PublishOrderPlaced(new OrderPlacedMessage
        {
            CorrelationId = order.CorrelationId,
            OrderId = order.Id,
            UserEmail = userEmail,
            UserName = userName,
            TotalAmount = order.TotalAmount,
            ShippingAddress = order.ShippingAddress,
            Items = order.Items.Select(i => new OrderItemMessage
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        });

        return order.Id;
    }

    public async Task UpdateStatusAsync(int id, UpdateStatusDto dto)
    {
        var order = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Order {id} not found");

        order.Status = dto.Status;
        if (dto.PaymentStatus != null) order.PaymentStatus = dto.PaymentStatus;
        await _repo.SaveChangesAsync();

        _publisher.PublishOrderStatusChanged(new OrderStatusChangedMessage
        {
            CorrelationId = order.CorrelationId,
            OrderId = order.Id,
            UserEmail = order.UserEmail,
            Status = order.Status,
            PaymentStatus = order.PaymentStatus
        });
    }

    private static OrderResponseDto Map(Order o) => new()
    {
        Id = o.Id,
        UserId = o.UserId,
        UserEmail = o.UserEmail,
        OrderDate = o.OrderDate,
        Status = o.Status,
        PaymentStatus = o.PaymentStatus,
        SagaState = o.SagaState,
        TotalAmount = o.TotalAmount,
        ShippingAddress = o.ShippingAddress,
        Items = o.Items.Select(i => new OrderItemResponseDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            Price = i.Price,
            Quantity = i.Quantity,
            Total = i.Price * i.Quantity
        }).ToList()
    };
}
