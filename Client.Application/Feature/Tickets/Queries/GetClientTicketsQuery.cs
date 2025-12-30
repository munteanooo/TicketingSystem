using Client.Application.Feature.Tickets.Commands.Ticket;
using MediatR;

namespace Client.Application.Feature.Tickets.Queries
{
    public class GetClientTicketsQuery : IRequest<List<TicketCommandResponseDto>>
    {
        public Guid ClientId { get; set; }
    }
}
