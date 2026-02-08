using MediatR;
using TicketingSystem.Application.Interfaces;

namespace TicketingSystem.Application.Tickets.Commands.ChangeStatus
{
    public record ChangeStatusCommand(ChangeStatusCommandDto CommandDto)
        : IRequest<ChangeStatusCommandResponseDto>, ICommand;
}