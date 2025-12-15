using Client.Application.Commands;
using MediatR;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Infrastructure.Data;

public class CloseTicketCommandHandler : IRequestHandler<CloseTicketCommand, TicketDto>
{
    private readonly ApplicationDbContext _context;

    public CloseTicketCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TicketDto> Handle(CloseTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets.FindAsync(new object[] { request.TicketId }, cancellationToken: cancellationToken);
        if (ticket == null)
            throw new Exception("Ticket not found");

        ticket.Status = TicketStatus.Closed;
        ticket.ResolvedAt = DateTime.UtcNow;
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
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            ResolvedAt = ticket.ResolvedAt
        };
    }
}