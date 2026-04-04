namespace CapShop.AuthService.Domain.Interfaces;

public interface IUserRepository
{
	Task<Domain.Entities.User?> GetByEmailAsync(string email);
	Task<bool> EmailExistsAsync(string email);
	Task AddAsync(Domain.Entities.User user);
	Task SaveChangesAsync();
}