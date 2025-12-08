using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;
using Client.Application.Commands;

namespace Client.Application.Handlers;

public class AddMessageToTicketCommandHandler : IRequestHandler<AddMessageToTicketCommand, int>
{
    private readonly ApplicationDbContext _context;

    public AddMessageToTicketCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(AddMessageToTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(t => t.Id == request.TicketId && t.CreatedByUserId == request.ClientId, cancellationToken);

        if (ticket == null)
            throw new UnauthorizedAccessException("Ticket not found or unauthorized");

        if (ticket.Status == TicketStatus.Closed || ticket.Status == TicketStatus.Resolved)
            throw new InvalidOperationException("Cannot add message to closed/resolved ticket");

        var message = new TicketMessage
        {
            Content = request.Content,
            TicketId = request.TicketId,
            UserId = request.ClientId,
            IsInternalNote = false,
            CreatedAt = DateTime.UtcNow
        };

        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.TicketMessages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return message.Id;
    }
}