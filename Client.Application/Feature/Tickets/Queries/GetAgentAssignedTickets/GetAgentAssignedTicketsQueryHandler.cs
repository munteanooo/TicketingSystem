using Client.Application.Contracts.Persistence;
using MediatR;

namespace Client.Application.Feature.Tickets.Queries.GetAgentAssignedTickets
{
    public class GetAgentAssignedTicketsQueryHandler
        : IRequestHandler<GetAgentAssignedTicketsQuery, List<GetAgentAssignedTicketsQueryResponseDto>>
    {
        private readonly ITicketRepository _ticketRepository;

        public GetAgentAssignedTicketsQueryHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<List<GetAgentAssignedTicketsQueryResponseDto>> Handle(
            GetAgentAssignedTicketsQuery request,
            CancellationToken cancellationToken)
        {
            var agentId = request.QueryDto.AgentId;

            var tickets = await _ticketRepository.GetByAgentIdAsync(agentId);

            return tickets.Select(t => new GetAgentAssignedTicketsQueryResponseDto
            {
                Id = t.Id,
                TicketNumber = t.TicketNumber,
                Title = t.Title,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                Category = t.Category,
                ClientId = t.ClientId,
                ClientName = t.Client?.FullName,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt ?? DateTime.MinValue,
                AssignedToAgentId = t.AssignedToAgentId
            }).ToList();
        }
    }
}
