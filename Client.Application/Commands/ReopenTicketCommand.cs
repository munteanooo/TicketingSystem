using MediatR;
using TicketingSystem.Application.DTOs;

namespace Client.Application.Commands
{
    public class ReopenTicketCommand : IRequest<TicketDto>
    {
        public Guid TicketId { get; set; }
        public string Reason { get; set; }
    }
}
