using MediatR;
using Client.Application.DTOs;

namespace Client.Application.Queries
{
    public class GetClientTicketsQuery : IRequest<List<TicketDto>>
    {
        public int ClientId { get; set; }
    }
}