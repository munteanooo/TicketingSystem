using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetByIdAsync(int id);
        Task<ApplicationUser> GetByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task AddAsync(ApplicationUser user);
        Task UpdateAsync(ApplicationUser user);
    }
}
