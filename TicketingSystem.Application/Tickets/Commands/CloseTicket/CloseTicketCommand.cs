using MediatR;

namespace TicketingSystem.Application.Tickets.Commands.CloseTicket
{
    public record CloseTicketCommand(CloseTicketCommandDto CommandDto)
        : IRequest<CloseTicketCommandResponseDto>;
}