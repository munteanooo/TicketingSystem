using Client.Application.Contracts.DTOs;

namespace Client.Application.Contracts.Services
{
    public interface ITokenService
    {
        Task<TokenPairDto> GenerateTokensAsync(UserDto user);
        Task<TokenPairDto> RefreshTokensAsync(string refreshToken);
        Task<bool> ValidateTokenAsync(string token);
        Task RevokeTokenAsync(string refreshToken);
        Task RevokeRefreshTokensForUserAsync(Guid userId);
        Task<UserDto?> GetUserFromTokenAsync(string token);
    }
}
