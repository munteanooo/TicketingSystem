using MediatR;

namespace TicketingSystem.Application.Tickets.Queries.GetTechTickets
{
    public record GetTechTicketsQuery(Guid TechnicianId)
        : IRequest<IEnumerable<GetTechTicketsQueryResponseDto>>;
}