using Client.Application.Contracts.Persistence;
using Client.Application.Feature.Tickets.Commands.Ticket;
using MediatR;
public class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, TicketCommandResponseDto>
{
    private readonly ITicketRepository _ticketRepository;

    public UpdateTicketCommandHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<TicketCommandResponseDto> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
    {
        var dto = request.TicketDto;

        // Obține ticket-ul din repository
        var ticket = await _ticketRepository.GetByIdAsync(dto.Id);
        if (ticket == null) throw new Exception("Ticket not found");

        // Actualizează proprietăți
        ticket.Title = dto.Title;
        ticket.Description = dto.Description;
        ticket.Priority = Enum.Parse<TicketingSystem.Domain.Enums.TicketPriority>(dto.Priority);
        ticket.Category = dto.Category;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _ticketRepository.UpdateAsync(ticket);

        // Mapare entitate → Response DTO
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
            ResolvedAt = ticket.ResolvedAt ?? DateTime.MinValue,
        };
    }
}
