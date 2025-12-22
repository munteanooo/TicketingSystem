namespace TicketingSystem.Application.Queries.Handlers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using TicketingSystem.Application.DTOs;
    using TicketingSystem.Infrastructure.Data;

    public class GetAllTicketsQueryHandler : IRequestHandler<GetAllTicketsQuery, List<TicketDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetAllTicketsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TicketDto>> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _context.Tickets
                .Include(t => t.Client)
                .Include(t => t.AssignedToAgent)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);

            return tickets.Select(MapToDto).ToList();
        }

        private TicketDto MapToDto(Domain.Entities.Ticket ticket)
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