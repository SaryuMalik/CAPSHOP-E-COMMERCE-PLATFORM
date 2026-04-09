using CapShop.OrderService.Application.DTOs;

namespace CapShop.OrderService.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(Guid userId);
    Task<OrderResponseDto?> GetByIdAsync(int id, Guid userId);
    Task<IEnumerable<OrderResponseDto>> GetAllAsync();
    Task<int> PlaceOrderAsync(PlaceOrderDto dto, Guid userId, string userEmail, string userName);
    Task UpdateStatusAsync(int id, UpdateStatusDto dto);
}
