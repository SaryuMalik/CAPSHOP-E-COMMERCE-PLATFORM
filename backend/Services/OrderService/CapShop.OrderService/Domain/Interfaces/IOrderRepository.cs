using CapShop.OrderService.Domain.Entities;

namespace CapShop.OrderService.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id, Guid userId);
    Task<Order?> GetByIdAsync(int id);
    Task<Order?> GetByCorrelationIdAsync(Guid correlationId);
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Order>> GetAllAsync();
    Task AddAsync(Order order);
    Task SaveChangesAsync();
}
