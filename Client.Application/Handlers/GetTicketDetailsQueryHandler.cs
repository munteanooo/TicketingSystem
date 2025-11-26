using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Infrastructure.Data;
using Client.Application.DTOs;
using Client.Application.Queries;

namespace Client.Application.Handlers
{
    public class GetTicketDetailsQueryHandler : IRequestHandler<GetTicketDetailsQuery, TicketDetailsDto>
    {
        private readonly ApplicationDbContext _context;

        public GetTicketDetailsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TicketDetailsDto> Handle(GetTicketDetailsQuery request, CancellationToken ct)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Messages)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(t => t.Id == request.TicketId && t.UserId == request.ClientId);

            if (ticket == null)
                return null;

            return new TicketDetailsDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.IsResolved ? "Resolved" : "Open",
                Priority = ticket.Priority.ToString(),
                Created = ticket.Created,
                IsResolved = ticket.IsResolved,
                Messages = ticket.Messages.Select(m => new TicketMessageDto
                {
                    Content = m.Content,
                    Created = m.Created,
                    AuthorName = m.User.FullName,
                    IsFromClient = m.UserId == request.ClientId
                }).ToList()
            };
        }
    }
}