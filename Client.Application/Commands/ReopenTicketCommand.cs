using MediatR;

namespace Client.Application.Commands
{
    public class ReopenTicketCommand : IRequest<Unit>
    {
        public int TicketId { get; set; }
        public int ClientId { get; set; }
    }
}