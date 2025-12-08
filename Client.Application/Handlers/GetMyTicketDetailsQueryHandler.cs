using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Infrastructure.Data;
using Client.Application.DTOs;
using Client.Application.Queries;

namespace Client.Application.Handlers
{
    public class GetMyTicketDetailsQueryHandler : IRequestHandler<GetTicketDetailsQuery, TicketDetailsDto?>
    {
        private readonly ApplicationDbContext _context;

        public GetMyTicketDetailsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TicketDetailsDto?> Handle(GetTicketDetailsQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _context.Tickets
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .Include(t => t.Messages)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(t =>
                    t.Id == request.TicketId &&
                    t.CreatedByUserId == request.ClientId,
                    cancellationToken);

            if (ticket == null)
                return null;

            return new TicketDetailsDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Priority = ticket.Priority,
                Status = ticket.Status,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                ClosedAt = ticket.ClosedAt,
                ClosingNotes = ticket.ClosingNotes,
                CreatedByUserId = ticket.CreatedByUserId,
                CreatedByUserName = ticket.CreatedByUser.Name,
                CreatedByUserEmail = ticket.CreatedByUser.Email,
                AssignedToUserId = ticket.AssignedToUserId,
                AssignedToUserName = ticket.AssignedToUser?.Name,
                AssignedToUserEmail = ticket.AssignedToUser?.Email,
                Messages = ticket.Messages.Select(m => new TicketMessageDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                    IsInternalNote = m.IsInternalNote,
                    UserId = m.UserId,
                    UserName = m.User.Name,
                    UserEmail = m.User.Email,
                    UserRole = m.User.Role.ToString()
                }).ToList()
            };
        }
    }
}