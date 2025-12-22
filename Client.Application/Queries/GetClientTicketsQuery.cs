using MediatR;
using TicketingSystem.Application.DTOs;

namespace Client.Application.Queries
{
    public class GetClientTicketsQuery : IRequest<List<TicketDto>>
    {
        public Guid ClientId { get; set; }
    }
}
