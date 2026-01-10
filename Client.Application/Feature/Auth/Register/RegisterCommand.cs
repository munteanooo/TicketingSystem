using Client.Application.Contracts.DTOs;
using MediatR;

namespace Client.Application.Feature.Auth.Register;

public record RegisterCommand(RegisterCommandDto RegisterDto)
    : IRequest<RegisterCommandResponseDto>;