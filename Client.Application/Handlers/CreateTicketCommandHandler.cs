using Client.Application.Commands;
using MediatR;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Infrastructure.Data;
using TicketingSystem.Domain.Enums;

namespace Client.Application.Handlers
{
    public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, int>  
    {
        private readonly ApplicationDbContext _context;

        public CreateTicketCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateTicketCommand request, CancellationToken ct)
        {
            var ticket = new Ticket
            {
                Title = request.Title,
                Description = request.Description,
                UserId = request.ClientId,
                Created = DateTime.UtcNow,
                Priority = TicketPriority.Medium,
                IsResolved = false
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync(ct);

            return ticket.Id;
        }
    }
}