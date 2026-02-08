using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Application.Interfaces;
using TicketingSystem.Application.Tickets.Queries.GetTechTickets;

namespace TicketingSystem.Application.Tickets.Queries.GetMyAssignedTickets;

public class GetMyAssignedTicketsQueryHandler : IRequestHandler<GetMyAssignedTicketsQuery, List<GetTechTicketsQueryResponseDto>>
{
    private readonly IApplicationDbContext _context;
    public GetMyAssignedTicketsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<GetTechTicketsQueryResponseDto>> Handle(GetMyAssignedTicketsQuery request, CancellationToken ct)
    {
        return await _context.Tickets
            .Include(t => t.Client)
            .Where(t => t.AssignedTechnicianId == request.TechnicianId && t.Status.ToString() != "Closed")
            .Select(t => new GetTechTicketsQueryResponseDto
            {
                Id = t.Id,
                TicketNumber = t.TicketNumber,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status.ToString(),
                Category = t.Category,
                Priority = t.Priority.ToString(),
                CreatedAt = t.CreatedAt,
                ClientId = t.ClientId,

                ClientName = t.Client != null ? t.Client.FullName : "Unknown Client"
            })
            .ToListAsync(ct);
    }
}