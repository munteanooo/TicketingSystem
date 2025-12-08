using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Domain.Enums;
using Client.Application.Commands;
using TicketingSystem.Infrastructure.Data;
using TicketingSystem.Infrastructure.Configurations;

namespace Client.Application.Handlers;

public class CloseTicketCommandHandler(ApplicationDbContext context)
    : IRequestHandler<CloseTicketCommand, Unit>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Unit> Handle(CloseTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(t => t.Id == request.TicketId && t.CreatedByUserId == request.ClientId, cancellationToken);

        if (ticket is null)
            throw new UnauthorizedAccessException("Ticket not found or unauthorized");

        if (ticket.Status == TicketStatus.Closed || ticket.Status == TicketStatus.Resolved)
            throw new InvalidOperationException("Ticket is already closed/resolved");

        ticket.Status = TicketStatus.Closed;
        ticket.ClosedAt = DateTime.UtcNow;
        ticket.ClosingNotes = request.ClosingNotes;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}