namespace TicketingSystem.Application.Contracts.Interfaces
{
    public interface ICurrentUser
    {
        string? UserId { get; }
        string? Email { get; }
        string? FullName { get; }
        string? UserRole { get; }

        bool IsClient { get; }
        bool IsTechnician { get; }
        bool IsAdmin { get; }

        bool IsAuthenticated { get; }
    }
}