using MediatR;
using Client.Application.Feature.Tickets.Commands.Ticket;
using System.Collections.Generic;

namespace Client.Application.Feature.Tickets.Queries.GetAllTickets
{
    public class GetAllTicketsQuery : IRequest<List<TicketCommandResponseDto>>
    {
        public GetAllTicketsQueryDto? Filters { get; set; }

        public GetAllTicketsQuery(GetAllTicketsQueryDto? filters = null)
        {
            Filters = filters;
        }
    }
}
