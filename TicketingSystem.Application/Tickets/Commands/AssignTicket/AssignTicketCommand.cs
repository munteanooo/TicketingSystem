using MediatR;
using TicketingSystem.Application.Interfaces;

namespace TicketingSystem.Application.Tickets.Commands.AssignTicket
{
    public record AssignTicketCommand(AssignTicketCommandDto CommandDto)
        : IRequest<AssignTicketCommandResponseDto>, ICommand;
}