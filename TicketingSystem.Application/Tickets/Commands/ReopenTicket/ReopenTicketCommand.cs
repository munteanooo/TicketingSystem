using MediatR;
using TicketingSystem.Application.Interfaces;

namespace TicketingSystem.Application.Tickets.Commands.ReopenTicket
{
    public record ReopenTicketCommand(ReopenTicketCommandDto CommandDto)
        : IRequest<ReopenTicketCommandResponseDto>, ICommand;
}