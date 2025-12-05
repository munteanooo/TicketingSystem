using Client.Application.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Infrastructure.Data;

namespace Client.Application.Handlers;

public class AddMessageToTicketCommandHandler(
    ApplicationDbContext context
    ) : IRequestHandler<AddMessageToTicketCommand, int>
{
    public async Task<int> Handle(AddMessageToTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await context.Tickets
            .FirstOrDefaultAsync(t => t.Id == request.TicketId && t.ClientId == request.ClientId, cancellationToken);

        if (ticket == null)
            throw new UnauthorizedAccessException("Ticket not found or unauthorized");

        if (ticket.Status == TicketStatus.Closed || ticket.Status == TicketStatus.Resolved)
            throw new InvalidOperationException("Cannot add message to closed/resolved ticket");

        var message = new TicketMessage
        {
            Content = request.Content,
            TicketId = request.TicketId,
            UserId = request.ClientId,
            IsInternal = false,
            CreatedAt = DateTime.UtcNow
        };

        ticket.UpdatedAt = DateTime.UtcNow;

        await context.TicketMessages.AddAsync(message, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return message.Id;
    }
}