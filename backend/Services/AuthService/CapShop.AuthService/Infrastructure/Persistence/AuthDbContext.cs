using Microsoft.EntityFrameworkCore;
using CapShop.AuthService.Domain.Entities;

namespace CapShop.AuthService.Infrastructure.Persistence
{
    public class AuthDbContext : DbContext
    {
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

        public DbSet<User> Users { get; set; }
    }
}
