using Client.Application.Contracts.Persistence;
using Client.Application.Feature.Tickets.Queries.GetClientTickets;
using MediatR;

public class GetClientTicketsQueryHandler
    : IRequestHandler<GetClientTicketsQuery, List<GetClientTicketsQueryResponseDto>>
{
    private readonly ITicketRepository _ticketRepository;

    public GetClientTicketsQueryHandler(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<List<GetClientTicketsQueryResponseDto>> Handle(
        GetClientTicketsQuery request,
        CancellationToken cancellationToken)
    {
        var tickets = await _ticketRepository.GetByUserAsync(request.Filters.ClientId);

        if (!string.IsNullOrEmpty(request.Filters.Status))
            tickets = tickets.Where(t => t.Status.ToString() == request.Filters.Status).ToList();

        if (!string.IsNullOrEmpty(request.Filters.Priority))
            tickets = tickets.Where(t => t.Priority.ToString() == request.Filters.Priority).ToList();

        if (request.Filters.Page.HasValue && request.Filters.PageSize.HasValue)
        {
            tickets = tickets
                .Skip((request.Filters.Page.Value - 1) * request.Filters.PageSize.Value)
                .Take(request.Filters.PageSize.Value)
                .ToList();
        }

        return tickets.Select(MapToDto).ToList();
    }

    private static GetClientTicketsQueryResponseDto MapToDto(TicketingSystem.Domain.Entities.Ticket ticket)
    {
        return new GetClientTicketsQueryResponseDto
        {
            Id = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            Title = ticket.Title,
            Description = ticket.Description,
            Priority = ticket.Priority.ToString(),
            Status = ticket.Status.ToString(),
            Category = ticket.Category,
            ClientId = ticket.ClientId,
            ClientName = ticket.Client?.FullName,
            AssignedToAgentId = ticket.AssignedToAgentId,
            AssignedToAgentName = ticket.AssignedToAgent?.FullName,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt ?? DateTime.MinValue
        };
    }
}