using CapShop.CatalogService.Domain.Entities;

namespace CapShop.CatalogService.Domain.Interfaces;

public interface IInventoryRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task SaveChangesAsync();
}
