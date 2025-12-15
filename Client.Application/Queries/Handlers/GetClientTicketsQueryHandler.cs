using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Infrastructure.Data;

namespace Client.Application.Queries.Handlers
{
    public class GetClientTicketsQueryHandler : IRequestHandler<GetClientTicketsQuery, List<TicketDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetClientTicketsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TicketDto>> Handle(GetClientTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _context.Tickets
                .Where(t => t.ClientId == request.ClientId)
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
