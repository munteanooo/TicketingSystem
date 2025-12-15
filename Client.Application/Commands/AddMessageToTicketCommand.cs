using MediatR;
using TicketingSystem.Application.DTOs;

namespace Client.Application.Commands
{
    public class AddMessageToTicketCommand : IRequest<TicketMessageDto>
    {
        public Guid TicketId { get; set; }
        public Guid AuthorId { get; set; }
        public string Content { get; set; }
    }
}
