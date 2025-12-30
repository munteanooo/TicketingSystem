using MediatR;

namespace Client.Application.Feature.Tickets.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginCommandResponseDto>;
