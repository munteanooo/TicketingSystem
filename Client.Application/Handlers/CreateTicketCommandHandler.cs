using MediatR;
using TicketingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;
using Client.Application.Commands;

namespace Client.Application.Handlers;

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, int>
{
    private readonly ApplicationDbContext _context;

    public CreateTicketCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var clientExists = await _context.Users
            .AnyAsync(u => u.Id == request.ClientId && u.Role == UserRole.Client, cancellationToken);

        if (!clientExists)
            throw new UnauthorizedAccessException("Client not found or unauthorized");

        var ticket = new Ticket
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            Status = TicketStatus.Open,
            CreatedByUserId = request.ClientId,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Tickets.AddAsync(ticket, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ticket.Id;
    }
}