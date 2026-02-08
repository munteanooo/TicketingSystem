using MediatR;
using TicketingSystem.Application.Tickets.Queries.GetTechTickets;

namespace TicketingSystem.Application.Tickets.Queries
{
    public record GetUnassignedTicketsQuery()
        : IRequest<IEnumerable<GetTechTicketsQueryResponseDto>>;
}