using MediatR;

namespace TicketingSystem.Application.Tickets.Queries.GetClientTickets
{
    public record GetClientTicketsQuery(Guid ClientId)
        : IRequest<IEnumerable<GetClientTicketsQueryResponseDto>>;
}