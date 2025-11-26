using Client.Application.Commands;
using MediatR;
using TicketingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Client.Application.Handlers
{
    public class ReopenTicketCommandHandler : IRequestHandler<ReopenTicketCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public ReopenTicketCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ReopenTicketCommand request, CancellationToken ct)
        {
            var ticket = await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == request.TicketId && t.UserId == request.ClientId);

            if (ticket != null)
            {
                ticket.IsResolved = false;
                await _context.SaveChangesAsync(ct);
            }

            return Unit.Value;
        }
    }
}