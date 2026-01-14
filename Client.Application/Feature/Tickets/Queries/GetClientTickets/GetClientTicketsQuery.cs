using Client.Application.Feature.Tickets.Queries.GetClientTickets;
using MediatR;
using TicketingSystem.Domain.Enums;

public class GetClientTicketsQuery
    : IRequest<List<GetClientTicketsQueryResponseDto>>
{
    public Guid ClientId { get; set; }

    public TicketStatus? Status { get; set; }
    public TicketPriority? Priority { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
