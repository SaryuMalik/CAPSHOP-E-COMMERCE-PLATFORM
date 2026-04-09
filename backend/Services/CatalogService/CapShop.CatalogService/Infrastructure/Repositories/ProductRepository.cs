using CapShop.CatalogService.Domain.Entities;
using CapShop.CatalogService.Domain.Interfaces;
using CapShop.CatalogService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CapShop.CatalogService.Infrastructure.Repositories;

public class ProductRepository : IInventoryRepository
{
    private readonly CatalogDbContext _db;

    public ProductRepository(CatalogDbContext db) => _db = db;

    public Task<Product?> GetByIdAsync(int id) =>
        _db.Products.FirstOrDefaultAsync(p => p.Id == id);

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
