using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Infrastructure.Data;

namespace Client.Application.Queries.Handlers
{
    public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDetailsDto>
    {
        private readonly ApplicationDbContext _context;

        public GetTicketByIdQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TicketDetailsDto> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Client)
                .Include(t => t.AssignedToAgent)
                .Include(t => t.Messages)
                .ThenInclude(m => m.Author)
                .FirstOrDefaultAsync(t => t.Id == request.TicketId, cancellationToken);

            if (ticket == null)
                throw new Exception("Ticket not found");

            return MapToDetailsDto(ticket);
        }

        private TicketDetailsDto MapToDetailsDto(TicketingSystem.Domain.Entities.Ticket ticket)
        {
            return new TicketDetailsDto
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
                ClientEmail = ticket.Client?.Email,
                AssignedToAgentId = ticket.AssignedToAgentId,
                AssignedToAgentName = ticket.AssignedToAgent?.FullName,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                ResolvedAt = ticket.ResolvedAt,
                Messages = ticket.Messages?.Select(m => new TicketMessageDto
                {
                    Id = m.Id,
                    TicketId = m.TicketId,
                    AuthorId = m.AuthorId,
                    AuthorName = m.Author?.FullName,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt
                }).ToList() ?? new List<TicketMessageDto>()
            };
        }
    }
}
