using Client.Application.Contracts.Persistence;
using MediatR;
using Client.Application.Feature.Tickets.Commands.Ticket;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;
using Client.Application.Feature.Tickets.Commands.UpdateStatus;

public class UpdateTicketStatusCommandHandler : IRequestHandler<UpdateTicketStatusCommand, TicketCommandResponseDto>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ITicketMessageRepository _ticketMessageRepository;

    public UpdateTicketStatusCommandHandler(
        ITicketRepository ticketRepository,
        ITicketMessageRepository ticketMessageRepository)
    {
        _ticketRepository = ticketRepository;
        _ticketMessageRepository = ticketMessageRepository;
    }

    public async Task<TicketCommandResponseDto> Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);
        if (ticket == null)
            throw new Exception("Ticket not found");

        ticket.Status = Enum.Parse<TicketStatus>(request.TicketStatusDto.NewStatus, ignoreCase: true);
        ticket.UpdatedAt = DateTime.UtcNow;
        if (ticket.Status == TicketStatus.Resolved)
            ticket.ResolvedAt = DateTime.UtcNow;

        await _ticketRepository.UpdateAsync(ticket);

        if (!string.IsNullOrEmpty(request.TicketStatusDto.Message))
        {
            var message = new TicketMessage
            {
                Id = Guid.NewGuid(),
                TicketId = ticket.Id,
                AuthorId = request.TicketStatusDto.UserId,
                Content = request.TicketStatusDto.Message,
                CreatedAt = DateTime.UtcNow
            };
            await _ticketMessageRepository.AddAsync(message);
        }

        return MapToDto(ticket);
    }

    private static TicketCommandResponseDto MapToDto(Ticket ticket)
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
            AssignedToAgentId = ticket.AssignedToAgentId,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt ?? DateTime.MinValue,
            ResolvedAt = ticket.ResolvedAt
        };
    }
}
