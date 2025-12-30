using MediatR;
using Client.Application.Feature.Tickets.Commands.Ticket;

namespace Client.Application.Feature.Tickets.Commands.UpdateStatus
{
    public class UpdateTicketStatusCommand : IRequest<TicketCommandResponseDto>
    {
        public Guid TicketId { get; set; }
        public string NewStatus { get; set; }
        public string Message { get; set; }
        public Guid UserId { get; set; }
    }
}
