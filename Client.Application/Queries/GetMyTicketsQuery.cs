using MediatR;
using Client.Application.DTOs;
using System.Collections.Generic;
using TicketingSystem.Domain.Enums;

namespace Client.Application.Queries;

public class GetMyTicketsQuery : IRequest<List<TicketDto>>
{
    public int ClientId { get; set; }
    public TicketStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool? IsClosed { get; set; }
}