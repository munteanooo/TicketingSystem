using Client.Application.Feature.Tickets.Commands.Ticket;
using MediatR;

namespace Client.Application.Feature.Tickets.Queries
{
    public class GetTicketMessagesQueryHandler : IRequest<List<TicketMessageDto>>
    {
        public Guid TicketId { get; set; }

        public GetTicketMessagesQueryHandler(Guid ticketId)
        {
            TicketId = ticketId;
        }
    }
}
