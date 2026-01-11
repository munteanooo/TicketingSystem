using MediatR;

namespace Client.Application.Feature.Auth.Register;

public record RegisterCommand(RegisterCommandDto RegisterDto)
    : IRequest<RegisterCommandResponseDto>;