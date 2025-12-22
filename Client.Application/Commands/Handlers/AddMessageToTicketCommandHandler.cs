using Client.Application.Commands;
using MediatR;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Infrastructure.Data;

public class AddMessageToTicketCommandHandler : IRequestHandler<AddMessageToTicketCommand, TicketMessageDto>
{
    private readonly ApplicationDbContext _context;

    public AddMessageToTicketCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TicketMessageDto> Handle(AddMessageToTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets.FindAsync(new object[] { request.TicketId }, cancellationToken: cancellationToken);
        if (ticket == null)
            throw new Exception("Ticket not found");

        var message = new TicketMessage
        {
            Id = Guid.NewGuid(),
            TicketId = request.TicketId,
            AuthorId = request.AuthorId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };

        ticket.UpdatedAt = DateTime.UtcNow;

        _context.TicketMessages.Add(message);
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(message);
    }

    private TicketMessageDto MapToDto(TicketMessage message)
    {
        return new TicketMessageDto
        {
            Id = message.Id,
            TicketId = message.TicketId,
            AuthorId = message.AuthorId,
            Content = message.Content,
            CreatedAt = message.CreatedAt
        };
    }
}