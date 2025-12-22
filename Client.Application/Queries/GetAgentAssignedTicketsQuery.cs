using MediatR;
using TicketingSystem.Application.DTOs;

namespace Client.Application.Queries
{
    public class GetAgentAssignedTicketsQuery : IRequest<List<TicketDto>>
    {
        public Guid AgentId { get; set; }
    }
}
