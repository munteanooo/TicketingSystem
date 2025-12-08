using Client.Application.DTOs;
using MediatR;
using TicketingSystem.Domain.Enums;

namespace Client.Application.Queries
{
    public class GetMyTicketsQuery : IRequest<List<TicketDto>>
    {
        public int ClientId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public TicketStatus? Status { get; set; } 
    }
}