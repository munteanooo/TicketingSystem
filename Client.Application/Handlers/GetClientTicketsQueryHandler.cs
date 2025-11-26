using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Infrastructure.Data;
using Client.Application.DTOs;
using Client.Application.Queries;

namespace Client.Application.Handlers
{
    public class GetClientTicketsQueryHandler : IRequestHandler<GetClientTicketsQuery, List<TicketDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetClientTicketsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TicketDto>> Handle(GetClientTicketsQuery request, CancellationToken ct)
        {
            var tickets = await _context.Tickets
                .Where(t => t.UserId == request.ClientId)
                .Select(t => new TicketDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.IsResolved ? "Resolved" : "Open",
                    Priority = t.Priority.ToString(),
                    Created = t.Created,
                    IsResolved = t.IsResolved
                })
                .ToListAsync(ct);

            return tickets;
        }
    }
}