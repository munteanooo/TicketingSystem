using MediatR;

namespace Client.Application.Feature.Auth.Login;

public record LoginCommand(LoginCommandDto LoginDto) 
    : IRequest<LoginCommandResponseDto>;