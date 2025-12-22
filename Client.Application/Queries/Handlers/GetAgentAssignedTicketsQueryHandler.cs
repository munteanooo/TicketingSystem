using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Infrastructure.Data;

namespace Client.Application.Queries.Handlers
{
    public class GetAgentAssignedTicketsQueryHandler : IRequestHandler<GetAgentAssignedTicketsQuery, List<TicketDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetAgentAssignedTicketsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TicketDto>> Handle(GetAgentAssignedTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _context.Tickets
                .Where(t => t.AssignedToAgentId == request.AgentId)
                .Include(t => t.Client)
                .Include(t => t.AssignedToAgent)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);

            return tickets.Select(MapToDto).ToList();
        }

        private TicketDto MapToDto(TicketingSystem.Domain.Entities.Ticket ticket)
        {
            return new TicketDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                Title = ticket.Title,
                Description = ticket.Description,
                Priority = ticket.Priority.ToString(),
                Status = ticket.Status.ToString(),
                Category = ticket.Category,
                ClientId = ticket.ClientId,
                ClientName = ticket.Client?.FullName,
                AssignedToAgentId = ticket.AssignedToAgentId,
                AssignedToAgentName = ticket.AssignedToAgent?.FullName,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt
            };
        }
    }
}
