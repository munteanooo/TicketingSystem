using Client.Application.Contracts.Services;
using Client.Application.Feature.Tickets.Commands.Login;
using Client.Application.Feature.Tickets.Commands.Register;
using MediatR;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, LoginCommandResponseDto>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService) 
    {
        _authService = authService;
    }

    public async Task<LoginCommandResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await _authService.RegisterAsync(request); 
    }
}
