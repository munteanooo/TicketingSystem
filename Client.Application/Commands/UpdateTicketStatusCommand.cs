using MediatR;
using TicketingSystem.Application.DTOs;

namespace Client.Application.Commands
{
    public class UpdateTicketStatusCommand : IRequest<TicketDto>
    {
        public Guid TicketId { get; set; }
        public string NewStatus { get; set; }
        public string Message { get; set; }
        public Guid UserId { get; set; }
    }
}
