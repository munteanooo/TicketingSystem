using MediatR;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Interfaces;

public class GetClientTicketsQuery : IRequest<List<TicketDto>>
{
    public int ClientId { get; set; }

    public GetClientTicketsQuery(int clientId)
    {
        ClientId = clientId;
    }
}

public class GetClientTicketsQueryHandler : IRequestHandler<GetClientTicketsQuery, List<TicketDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetClientTicketsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TicketDto>> Handle(GetClientTicketsQuery request, CancellationToken cancellationToken)
    {
        var allTickets = await _unitOfWork.Tickets.GetAllAsync();
        var clientTickets = allTickets.Where(t => t.CreatedById == request.ClientId);
        return clientTickets.Select(MapToDto).ToList();
    }

    private TicketDto MapToDto(Ticket ticket)
    {
        return new TicketDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Priority = ticket.Priority,
            Status = ticket.Status,
            CreatedById = ticket.CreatedById,
            AssignedToId = ticket.AssignedToId,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            ResolvedAt = ticket.ResolvedAt
        };
    }
}