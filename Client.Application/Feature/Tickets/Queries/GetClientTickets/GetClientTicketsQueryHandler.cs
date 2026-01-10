using Client.Application.Contracts.Persistence;
using Client.Application.Feature.Tickets.Queries.GetClientTickets;
using MediatR;
using TicketingSystem.Domain.Entities;

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
        var tickets = await _ticketRepository.GetByUserAsync(request.ClientId);

        if (request.Status.HasValue)
        {
            tickets = tickets
                .Where(t => t.Status == request.Status.Value)
                .ToList();
        }

        if (request.Priority.HasValue)
        {
            tickets = tickets
                .Where(t => t.Priority == request.Priority.Value)
                .ToList();
        }

        tickets = tickets
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return tickets.Select(MapToDto).ToList();
    }

    private static GetClientTicketsQueryResponseDto MapToDto(Ticket ticket)
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
            UpdatedAt = ticket.UpdatedAt ?? ticket.CreatedAt
        };
    }
}
