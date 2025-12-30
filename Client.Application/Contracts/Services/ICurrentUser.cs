namespace Client.Application.Contracts.Services
{
    public interface ICurrentUser
    {
        int? UserId { get; }
        string? Email { get; }
        string? FullName { get; }
        bool IsAuthenticated { get; }
    }
}