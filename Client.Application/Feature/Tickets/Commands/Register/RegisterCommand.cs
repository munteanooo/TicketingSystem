using MediatR;

namespace Client.Application.Feature.Tickets.Commands.Register;

public record RegisterCommand(RegisterCommandDto RegisterDto)
    : IRequest<RegisterCommandResponseDto>;
