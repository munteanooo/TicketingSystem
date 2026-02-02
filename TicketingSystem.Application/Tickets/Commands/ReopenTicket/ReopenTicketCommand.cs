using MediatR;

namespace TicketingSystem.Application.Tickets.Commands.ReopenTicket
{
    public record ReopenTicketCommand(ReopenTicketCommandDto CommandDto)
        : IRequest<ReopenTicketCommandResponseDto>;
}