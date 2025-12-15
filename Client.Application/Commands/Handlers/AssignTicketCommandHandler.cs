using MediatR;
using TicketingSystem.Application.Commands;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Infrastructure.Data;

namespace Client.Application.Commands.Handlers
{
    public class AssignTicketCommandHandler : IRequestHandler<AssignTicketCommand, TicketDto>
    {
        private readonly ApplicationDbContext _context;

        public AssignTicketCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TicketDto> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _context.Tickets.FindAsync(new object[] { request.TicketId }, cancellationToken: cancellationToken);
            if (ticket == null)
                throw new Exception("Ticket not found");

            ticket.AssignedToAgentId = request.AgentId;
            ticket.Status = TicketStatus.InProgress;
            ticket.UpdatedAt = DateTime.UtcNow;

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync(cancellationToken);

            return MapToDto(ticket);
        }

        private TicketDto MapToDto(Ticket ticket)
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
                AssignedToAgentId = ticket.AssignedToAgentId,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt
            };
        }
    }
}
