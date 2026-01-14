using Client.Application.Contracts.DTOs;
using Client.Application.Feature.Auth.Login;
using Client.Application.Feature.Auth.Register;

namespace Client.Application.Contracts.Services
{
    public interface IAuthService
    {
        Task<RegisterCommandResponseDto> RegisterAsync(RegisterCommandDto registerDto);
        Task<LoginCommandResponseDto> LoginAsync(LoginCommandDto loginDto);
        Task<LoginCommandResponseDto> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(Guid userId);
        Task<UserDto?> GetUserByIdAsync(Guid userId);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<bool> ValidateCredentialsAsync(string email, string password);
    }
}
