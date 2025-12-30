using Client.Application.Contracts.Persistence;
using Client.Application.Feature.Tickets.Commands.Ticket;
using MediatR;

namespace Client.Application.Feature.Tickets.Queries.Handlers
{
    public class GetAgentAssignedTicketsQueryHandler : IRequestHandler<GetAgentAssignedTicketsQuery, List<TicketCommandResponseDto>>
    {
        private readonly ITicketRepository _ticketRepository;

        public GetAgentAssignedTicketsQueryHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<List<TicketCommandResponseDto>> Handle(GetAgentAssignedTicketsQuery request, CancellationToken cancellationToken)
        {
            // Folosim metoda repository-ului pentru a obține toate tichetele atribuite agentului
            var tickets = await _ticketRepository.GetByAgentIdAsync(request.AgentId);

            return tickets.Select(MapToDto).ToList();
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
                ClientName = ticket.Client?.FullName,
                AssignedToAgentId = ticket.AssignedToAgentId,
                AssignedToAgentName = ticket.AssignedToAgent?.FullName,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt ?? DateTime.MinValue
            };
        }
    }
}
