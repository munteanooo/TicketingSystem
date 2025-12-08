using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Domain.Enums;
using Client.Application.Commands;
using TicketingSystem.Infrastructure.Data;

namespace Client.Application.Handlers;

public class ReopenTicketCommandHandler(ApplicationDbContext context)
    : IRequestHandler<ReopenTicketCommand, Unit>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Unit> Handle(ReopenTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(t => t.Id == request.TicketId && t.CreatedByUserId == request.ClientId, cancellationToken);

        if (ticket is null)
            throw new UnauthorizedAccessException("Ticket not found or unauthorized");

        if (ticket.Status != TicketStatus.Closed && ticket.Status != TicketStatus.Resolved)
            throw new InvalidOperationException("Only closed/resolved tickets can be reopened");

        ticket.Status = TicketStatus.Reopened;
        ticket.ClosedAt = null;
        ticket.ClosingNotes = string.Empty;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}