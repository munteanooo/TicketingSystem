using MediatR;
using TicketingSystem.Application.DTOs;

namespace Client.Application.Commands
{
    public class CloseTicketCommand : IRequest<TicketDto>
    {
        public Guid TicketId { get; set; }
    }
}
