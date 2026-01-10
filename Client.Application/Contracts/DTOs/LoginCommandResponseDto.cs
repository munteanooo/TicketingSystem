namespace Client.Application.Contracts.DTOs
{
    public class LoginCommandResponseDto
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty; 
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; } = null!; 
        public string Message { get; set; } = string.Empty;
    }
}