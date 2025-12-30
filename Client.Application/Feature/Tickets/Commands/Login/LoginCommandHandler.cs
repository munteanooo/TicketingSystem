using MediatR;
using Client.Application.Contracts.Services;
using Client.Application.Feature.Tickets.Commands.Ticket;

namespace Client.Application.Feature.Tickets.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResponseDto>
{
    private readonly IAuthService _authService;

    public LoginCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<LoginCommandResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _authService.LoginAsync(request);
    }
}
