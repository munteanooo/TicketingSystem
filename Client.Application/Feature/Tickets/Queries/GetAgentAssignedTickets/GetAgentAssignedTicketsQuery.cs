using MediatR;

namespace Client.Application.Feature.Tickets.Queries.GetAgentAssignedTickets;

public record GetAgentAssignedTicketsQuery(GetAgentAssignedTicketsQueryDto QueryDto)
    : IRequest<List<GetAgentAssignedTicketsQueryResponseDto>>;
