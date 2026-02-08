namespace TicketingSystem.Blazor.Services.Interfaces;

public interface IAuthService
{
    Task<bool> Login(string email, string password);
    Task<bool> Register(string fullName, string email, string password, string role);
    Task Logout();
}