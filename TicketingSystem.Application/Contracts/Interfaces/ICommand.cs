namespace TicketingSystem.Application.Interfaces
{
    /// <summary>
    /// Marker interface for transactional commands.
    /// Handlers for requests implementing this interface will have
    /// DbContext.SaveChangesAsync invoked automatically by the UnitOfWorkBehavior.
    /// </summary>
    public interface ICommand { }
}
