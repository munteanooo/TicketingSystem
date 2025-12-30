using Client.Application.Feature.Tickets.Commands.Login;
using Client.Application.Feature.Tickets.Commands.Register;

namespace Client.Application.Contracts.Services
{
    public interface IAuthService
    {
        Task<RegisterCommandResponseDto> RegisterAsync(RegisterCommand command);
        Task<LoginCommandResponseDto> LoginAsync(LoginCommand command);
        Task<LoginCommandResponseDto> RefreshTokenAsync(Guid userId);
        Task LogoutAsync(Guid userId);
        Task<LoginCommandDto?> GetUserByIdAsync(Guid userId);
    }
}
