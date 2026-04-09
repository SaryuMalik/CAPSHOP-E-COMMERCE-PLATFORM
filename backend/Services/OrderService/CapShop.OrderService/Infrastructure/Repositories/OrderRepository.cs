using CapShop.OrderService.Domain.Entities;
using CapShop.OrderService.Domain.Interfaces;
using CapShop.OrderService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CapShop.OrderService.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _db;

    public OrderRepository(OrderDbContext db) => _db = db;

    public Task<Order?> GetByIdAsync(int id, Guid userId) =>
        _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

    public Task<Order?> GetByIdAsync(int id) =>
        _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);

    public Task<Order?> GetByCorrelationIdAsync(Guid correlationId) =>
        _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.CorrelationId == correlationId);

    public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId) =>
        await _db.Orders.Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OrderDate).ToListAsync();

    public async Task<IEnumerable<Order>> GetAllAsync() =>
        await _db.Orders.Include(o => o.Items)
            .OrderByDescending(o => o.OrderDate).ToListAsync();

    public async Task AddAsync(Order order) => await _db.Orders.AddAsync(order);

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
