namespace TicketingSystem.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITicketRepository Tickets { get; }
        IUserRepository Users { get; }
        Task SaveChangesAsync();
    }
}
