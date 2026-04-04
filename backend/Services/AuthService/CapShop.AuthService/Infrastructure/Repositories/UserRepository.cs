using CapShop.AuthService.Domain.Entities;
using CapShop.AuthService.Domain.Interfaces;
using CapShop.AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CapShop.AuthService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<bool> EmailExistsAsync(string email)
        => await _context.Users.AnyAsync(u => u.Email == email);

    public async Task AddAsync(User user)
        => await _context.Users.AddAsync(user);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}