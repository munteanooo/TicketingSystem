using MediatR;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.Interfaces;

public class GetOpenTicketsQuery : IRequest<List<TicketDto>>
{
}

public class GetOpenTicketsQueryHandler : IRequestHandler<GetOpenTicketsQuery, List<TicketDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOpenTicketsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TicketDto>> Handle(GetOpenTicketsQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _unitOfWork.Tickets.GetByStatusAsync(TicketStatus.Open);
        return tickets.Select(MapToDto).ToList();
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