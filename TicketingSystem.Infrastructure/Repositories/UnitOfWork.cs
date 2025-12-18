using TicketingSystem.Domain.Interfaces;
using TicketingSystem.Infrastructure.Data;
using TicketingSystem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private ITicketRepository _ticketRepository;
    private IUserRepository _userRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public ITicketRepository Tickets
    {
        get { return _ticketRepository ??= new TicketRepository(_context); }
    }

    public IUserRepository Users
    {
        get { return _userRepository ??= new UserRepository(_context); }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}