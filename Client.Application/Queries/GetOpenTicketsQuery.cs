using MediatR;
using TicketingSystem.Application.DTOs;

namespace Client.Application.Queries
{
    public class GetOpenTicketsQuery : IRequest<List<TicketDto>>
    {
    }
}
