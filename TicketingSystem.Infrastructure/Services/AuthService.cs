using Client.Application.Contracts.Services;
using Client.Application.Feature.Tickets.Commands.Login;
using Client.Application.Feature.Tickets.Commands.Register;
using Microsoft.AspNetCore.Identity;
using TicketingSystem.Infrastructure.Identity;

namespace TicketingSystem.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<RegisterCommandResponseDto> RegisterAsync(RegisterCommand command)
        {
            var dto = command.RegisterDto;

            // Aici vine logica reală de creare user în Identity
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber
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

            // Poți adăuga aici logică de roluri, token etc.

            return new RegisterCommandResponseDto
            {
                Success = true,
                Message = "User registered successfully",
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName
            };
        }

        public async Task<LoginCommandResponseDto> LoginAsync(LoginCommand command)
        {
            throw new NotImplementedException();
        }

        public async Task<LoginCommandResponseDto> RefreshTokenAsync(Guid userId)
        {
            return await Task.FromResult(new LoginCommandResponseDto
            {
                Success = false,
                Message = "Not implemented"
            });
        }

        public async Task LogoutAsync(Guid userId)
        {
            await Task.CompletedTask;
        }

        public async Task<LoginCommandDto?> GetUserByIdAsync(Guid userId)
        {
            return await Task.FromResult<LoginCommandDto?>(null);
        }
    }
}
