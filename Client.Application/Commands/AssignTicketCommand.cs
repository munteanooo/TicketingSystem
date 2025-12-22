using MediatR;
using TicketingSystem.Application.DTOs;

namespace TicketingSystem.Application.Commands
{
    public class AssignTicketCommand : IRequest<TicketDto>
    {
        public Guid TicketId { get; set; }
        public Guid AgentId { get; set; }
    }
}