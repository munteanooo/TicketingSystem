using Client.Application.Contracts.DTOs;
using Client.Application.Contracts.Services;
using Client.Application.Feature.Auth.Login;
using Client.Application.Feature.Auth.Register;
using Microsoft.AspNetCore.Identity;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Infrastructure.Identity;

namespace TicketingSystem.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<RegisterCommandResponseDto> RegisterAsync(RegisterCommandDto dto)
        {
            // Validare parolă
            if (dto.Password != dto.ConfirmPassword)
            {
                return new RegisterCommandResponseDto
                {
                    Success = false,
                    Message = "Passwords do not match"
                };
            }

            // Verifică dacă userul există deja
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return new RegisterCommandResponseDto
                {
                    Success = false,
                    Message = "User with this email already exists"
                };
            }

            // Creează user
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = $"{dto.FirstName} {dto.LastName}",
                Role = UserRole.Client,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                return new RegisterCommandResponseDto
                {
                    Success = false,
                    Message = string.Join("; ", result.Errors.Select(e => e.Description))
                };
            }

            // Adaugă userul în rolul Client
            await _userManager.AddToRoleAsync(user, UserRole.Client.ToString());

            // Creează UserDto pentru token
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive,
                PhoneNumber = user.PhoneNumber
            };

            // Generează tokens
            var tokenPair = await _tokenService.GenerateTokensAsync(userDto);

            return new RegisterCommandResponseDto
            {
                Success = true,
                Message = "User registered successfully",
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Role = user.Role,
                Token = tokenPair.AccessToken,
                RefreshToken = tokenPair.RefreshToken
            };
        }

        public async Task<LoginCommandResponseDto> LoginAsync(LoginCommandDto dto)
        {
            // Găsește user
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return new LoginCommandResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password",
                    User = new UserDto()
                };
            }

            // Verifică parola
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
            {
                return new LoginCommandResponseDto
                {
                    Success = false,
                    Message = "Invalid email or password",
                    User = new UserDto()
                };
            }

            // Verifică dacă userul este activ
            if (!user.IsActive)
            {
                return new LoginCommandResponseDto
                {
                    Success = false,
                    Message = "Account is deactivated",
                    User = new UserDto()
                };
            }

            // Creează UserDto
            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive,
                PhoneNumber = user.PhoneNumber
            };

            // Generează tokens
            var tokenPair = await _tokenService.GenerateTokensAsync(userDto);

            return new LoginCommandResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = tokenPair.AccessToken,
                RefreshToken = tokenPair.RefreshToken,
                ExpiresAt = tokenPair.AccessTokenExpiry,
                User = userDto
            };
        }

        public async Task<LoginCommandResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var tokenPair = await _tokenService.RefreshTokensAsync(refreshToken);

            return new LoginCommandResponseDto
            {
                Success = true,
                Message = "Token refreshed successfully",
                Token = tokenPair.AccessToken,
                RefreshToken = tokenPair.RefreshToken,
                ExpiresAt = tokenPair.AccessTokenExpiry,
                User = tokenPair.User
            };
        }

        public async Task LogoutAsync(Guid userId)
        {
            await _signInManager.SignOutAsync();
            await _tokenService.RevokeRefreshTokensForUserAsync(userId);
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            return await _userManager.CheckPasswordAsync(user, password);
        }
    }
}
