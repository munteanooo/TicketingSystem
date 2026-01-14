using Client.Application.Contracts.Services;
using MediatR;

namespace Client.Application.Feature.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterCommandResponseDto>
    {
        private readonly IAuthService _authService;

        public RegisterCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<RegisterCommandResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(request.RegisterDto);

            return result;
        }
    }
}
