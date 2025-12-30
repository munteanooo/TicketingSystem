using MediatR;
using Client.Application.Feature.Tickets.Commands.Ticket;

namespace Client.Application.Feature.Tickets.Commands.Reopen
{
    public class ReopenTicketCommand : IRequest<TicketCommandResponseDto>
    {
        public Guid TicketId { get; set; }
        public required string Reason { get; set; }
    }
}
