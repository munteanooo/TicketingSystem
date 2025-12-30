using Client.Application.Feature.Tickets.Commands.Ticket;
using MediatR;

namespace Client.Application.Feature.Tickets.Queries
{
    public class GetAgentAssignedTicketsQuery : IRequest<List<TicketCommandResponseDto>>
    {
        public Guid AgentId { get; set; }
    }
}
