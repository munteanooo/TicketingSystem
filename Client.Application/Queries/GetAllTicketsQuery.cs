namespace TicketingSystem.Application.Queries
{
    using System.Collections.Generic;
    using MediatR;
    using TicketingSystem.Application.DTOs;

    public class GetAllTicketsQuery : IRequest<List<TicketDto>>
    {
    }
}