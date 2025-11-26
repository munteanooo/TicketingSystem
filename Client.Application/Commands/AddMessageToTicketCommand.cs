using MediatR;

namespace Client.Application.Commands
{
    public class AddMessageToTicketCommand : IRequest<Unit>
    {
        public int TicketId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int ClientId { get; set; }
    }
}