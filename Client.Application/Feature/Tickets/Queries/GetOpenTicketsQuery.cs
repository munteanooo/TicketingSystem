using Client.Application.Feature.Tickets.Commands.Ticket;
using MediatR;

namespace Client.Application.Feature.Tickets.Queries
{
    public class GetOpenTicketsQuery : IRequest<List<TicketCommandResponseDto>>
    {
    }
}
