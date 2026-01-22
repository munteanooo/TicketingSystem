using Client.Application.Contracts.DTOs;

namespace Client.Application.Feature.Auth.Login
{
    public class LoginCommandResponseDto
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty; 
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; } = null!; 
        public string Message { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}