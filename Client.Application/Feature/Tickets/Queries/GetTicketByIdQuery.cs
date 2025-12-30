using Client.Application.Feature.Tickets.Commands.Ticket;
using MediatR;

namespace Client.Application.Feature.Tickets.Queries
{
    public class GetTicketByIdQuery : IRequest<TicketCommandResponseDto>
    {
        public Guid TicketId { get; set; }
    }
}
