namespace Client.Application.Contracts.Services
{
    public interface ICurrentUser
    {
        Guid? UserId { get; }
        string? Email { get; }
        string? FullName { get; }
        bool IsAuthenticated { get; }
    }
}