using TicketingSystem.Application.Commands;

namespace Client.Application.Commands.Handlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using TicketingSystem.Application.DTOs;
    using TicketingSystem.Domain.Entities;
    using TicketingSystem.Domain.Enums;
    using TicketingSystem.Infrastructure.Data;

    public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, TicketDto>
    {
        private readonly ApplicationDbContext _context;

        public CreateTicketCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TicketDto> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                TicketNumber = GenerateTicketNumber(),
                Title = request.Ticket.Title,
                Description = request.Ticket.Description,
                Category = request.Ticket.Category,
                Priority = Enum.Parse<TicketPriority>(request.Ticket.Priority, ignoreCase: true),
                Status = TicketStatus.Open,
                ClientId = request.ClientId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync(cancellationToken);

            return MapToDto(ticket);
        }

        private string GenerateTicketNumber()
        {
            var year = DateTime.UtcNow.Year;
            var random = new Random();
            return $"TK-{year}-{random.Next(10000, 99999)}";
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
                CreatedAt = ticket.CreatedAt
            };
        }
    }
}