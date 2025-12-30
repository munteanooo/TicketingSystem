namespace Client.Application.Feature.Tickets.Commands.Register;

public class RegisterCommandResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? UserId { get; set; } 
    public string? Email { get; set; }
    public string? FullName { get; set; }
}
