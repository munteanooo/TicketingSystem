using MediatR;
using TicketingSystem.Application.DTOs;

namespace Client.Application.Queries
{
    public class GetTicketMessagesQuery : IRequest<List<TicketMessageDto>>
    {
        public Guid TicketId { get; set; }
    }
}
