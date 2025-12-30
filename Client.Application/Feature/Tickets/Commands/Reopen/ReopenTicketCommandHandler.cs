using Client.Application.Contracts.Persistence;
using Client.Application.Feature.Tickets.Commands.Reopen;
using MediatR;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;

public class ReopenTicketCommandHandler : IRequestHandler<ReopenTicketCommand, ReopenTicketCommandResponseDto>
{
    private readonly ITicketRepository _ticketRepository;

    public ReopenTicketCommandHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<ReopenTicketCommandResponseDto> Handle(ReopenTicketCommand request, CancellationToken cancellationToken)
    {
        var dto = request.ReopenDto;

        var ticket = await _ticketRepository.GetByIdAsync(dto.TicketId);
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
            Content = dto.Reason,
            CreatedAt = DateTime.UtcNow
        };

        await _ticketRepository.UpdateAsync(ticket);

        if (_ticketRepository is ITicketMessageRepository messageRepo)
        {
            await messageRepo.AddAsync(message);
        }

        return MapToDto(ticket);
    }

    private static ReopenTicketCommandResponseDto MapToDto(Ticket ticket)
    {
        return new ReopenTicketCommandResponseDto
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
            UpdatedAt = ticket.UpdatedAt ?? DateTime.MinValue,
            ResolvedAt = ticket.ResolvedAt
        };
    }
}
