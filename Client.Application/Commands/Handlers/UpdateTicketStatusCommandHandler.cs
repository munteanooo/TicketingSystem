using Client.Application.Commands;
using MediatR;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Infrastructure.Data;

public class UpdateTicketStatusCommandHandler : IRequestHandler<UpdateTicketStatusCommand, TicketDto>
{
    private readonly ApplicationDbContext _context;

    public UpdateTicketStatusCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TicketDto> Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets.FindAsync(new object[] { request.TicketId }, cancellationToken: cancellationToken);
        if (ticket == null)
            throw new Exception("Ticket not found");

        ticket.Status = Enum.Parse<TicketStatus>(request.NewStatus, ignoreCase: true);
        ticket.UpdatedAt = DateTime.UtcNow;

        if (ticket.Status == TicketStatus.Resolved)
            ticket.ResolvedAt = DateTime.UtcNow;

        _context.Tickets.Update(ticket);

        if (!string.IsNullOrEmpty(request.Message))
        {
            var message = new TicketMessage
            {
                Id = Guid.NewGuid(),
                TicketId = request.TicketId,
                AuthorId = request.UserId,
                Content = request.Message,
                CreatedAt = DateTime.UtcNow
            };

            _context.TicketMessages.Add(message);
        }

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
            AssignedToAgentId = ticket.AssignedToAgentId,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            ResolvedAt = ticket.ResolvedAt
        };
    }
}