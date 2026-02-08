using MediatR;
using TicketingSystem.Application.Tickets.Queries.GetTechTickets;

namespace TicketingSystem.Application.Tickets.Queries;

public record GetMyAssignedTicketsQuery(Guid TechnicianId) : 
    IRequest<List<GetTechTicketsQueryResponseDto>>;