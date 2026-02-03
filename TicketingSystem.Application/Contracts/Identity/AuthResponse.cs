namespace TicketingSystem.Application.Contracts.Identity;

public class AuthResponse
{
    public bool IsSuccess { get; set; }
    public string Token { get; set; } = string.Empty;
    public string? Error { get; set; }
}