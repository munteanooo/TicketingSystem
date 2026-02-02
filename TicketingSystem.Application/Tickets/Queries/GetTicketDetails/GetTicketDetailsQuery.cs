using MediatR;

namespace TicketingSystem.Application.Tickets.Queries.GetTicketDetails
{
    public record GetTicketDetailsQuery(Guid TicketId)
        : IRequest<GetTicketDetailsQueryResponseDto>;
}