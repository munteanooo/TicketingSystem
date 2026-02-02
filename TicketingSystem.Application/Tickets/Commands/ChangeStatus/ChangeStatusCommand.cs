using MediatR;

namespace TicketingSystem.Application.Tickets.Commands.ChangeStatus
{
    public record ChangeStatusCommand(ChangeStatusCommandDto CommandDto)
        : IRequest<ChangeStatusCommandResponseDto>;
}