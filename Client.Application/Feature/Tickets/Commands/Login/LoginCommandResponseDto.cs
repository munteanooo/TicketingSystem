namespace Client.Application.Feature.Tickets.Commands.Login;
    public class LoginCommandResponseDto
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string Message { get; set; } = string.Empty;
    }
