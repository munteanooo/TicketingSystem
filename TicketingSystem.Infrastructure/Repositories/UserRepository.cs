using Microsoft.EntityFrameworkCore;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Interfaces;
using TicketingSystem.Infrastructure.Data;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationUser> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.CreatedTickets)
            .Include(u => u.AssignedTickets)
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
    }

    public async Task<ApplicationUser> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
    {
        return await _context.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.FirstName)
            .ToListAsync();
    }

    public async Task AddAsync(ApplicationUser user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task UpdateAsync(ApplicationUser user)
    {
        _context.Users.Update(user);
        await Task.CompletedTask;
    }
}