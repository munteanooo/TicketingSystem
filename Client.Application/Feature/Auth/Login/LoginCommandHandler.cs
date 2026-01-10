using Client.Application.Contracts.DTOs;
using Client.Application.Contracts.Services;
using MediatR;

namespace Client.Application.Feature.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResponseDto>
    {
        private readonly IAuthService _authService;

        public LoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<LoginCommandResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _authService.LoginAsync(request.LoginDto);
        }
    }
}