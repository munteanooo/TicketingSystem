using Client.Application.Feature.Tickets.Commands.Ticket;
using MediatR;

namespace Client.Application.Feature.Tickets.Queries
{
    public class GetTicketMessagesQuery : IRequest<List<TicketCommandResponseDto>>
    {
        public Guid TicketId { get; set; }
    }
}
