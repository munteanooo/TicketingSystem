using MediatR;
using TicketingSystem.Application.DTOs;

namespace Client.Application.Queries
{
    public class GetTicketByIdQuery : IRequest<TicketDetailsDto>
    {
        public Guid TicketId { get; set; }
    }
}
