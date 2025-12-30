namespace Client.Application.Feature.Tickets.Queries
{
    using System.Collections.Generic;
    using Client.Application.Feature.Tickets.Commands.Ticket;
    using MediatR;

    public class GetAllTicketsQuery : IRequest<List<TicketCommandResponseDto>>
    {
    }
}