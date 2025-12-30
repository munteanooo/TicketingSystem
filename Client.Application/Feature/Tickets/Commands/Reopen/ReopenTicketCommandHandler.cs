using Client.Application.Contracts.Persistence;
using MediatR;
using Client.Application.Feature.Tickets.Commands.Ticket;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;
using Client.Application.Feature.Tickets.Commands.Reopen;

public class ReopenTicketCommandHandler : IRequestHandler<ReopenTicketCommand, TicketCommandResponseDto>
{
    private readonly ITicketRepository _ticketRepository;

    public ReopenTicketCommandHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<TicketCommandResponseDto> Handle(ReopenTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);
        if (ticket == null)
            throw new Exception("Ticket not found");

        ticket.Status = TicketStatus.Reopened;
        ticket.UpdatedAt = DateTime.UtcNow;
        ticket.ResolvedAt = null;

        var message = new TicketMessage
        {
            Id = Guid.NewGuid(),
            TicketId = ticket.Id,
            AuthorId = ticket.ClientId,
            Content = request.Reason,
            CreatedAt = DateTime.UtcNow
        };

        // Folosim repository-ul pentru update
        await _ticketRepository.UpdateAsync(ticket);

        // Dacă repository-ul are metodă pentru mesaj, folosim add
        if (_ticketRepository is ITicketMessageRepository messageRepo)
        {
            await messageRepo.AddAsync(message);
        }

        return MapToDto(ticket);
    }

    private TicketCommandResponseDto MapToDto(TicketingSystem.Domain.Entities.Ticket ticket)
    {
        return new TicketCommandResponseDto
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
            UpdatedAt = ticket.UpdatedAt ?? DateTime.MinValue,
        };
    }
}
