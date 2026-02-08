using MediatR;
using TicketingSystem.Application.Interfaces;

namespace TicketingSystem.Application.Tickets.Commands.CloseTicket
{
    public record CloseTicketCommand(CloseTicketCommandDto CommandDto)
        : IRequest<CloseTicketCommandResponseDto>, ICommand;
}