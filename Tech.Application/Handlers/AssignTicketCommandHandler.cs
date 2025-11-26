using MediatR;
using TicketingSystem.Infrastructure.Data;
using Tech.Application.Commands;
using Microsoft.EntityFrameworkCore;

namespace Tech.Application.Handlers
{
    public class AssignTicketCommandHandler : IRequestHandler<AssignTicketCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public AssignTicketCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AssignTicketCommand request, CancellationToken ct)
        {
            var ticket = await _context.Tickets.FindAsync(request.TicketId);

            if (ticket != null)
            {
                ticket.TechSupportId = request.TechSupportId;
                await _context.SaveChangesAsync(ct);
            }

            return Unit.Value;
        }
    }
}