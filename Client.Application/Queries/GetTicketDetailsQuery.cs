using MediatR;
using Client.Application.DTOs;

namespace Client.Application.Queries
{
    public class GetTicketDetailsQuery : IRequest<TicketDetailsDto>
    {
        public int TicketId { get; set; }
        public int ClientId { get; set; }
    }
}