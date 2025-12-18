using MediatR;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.Interfaces;

public class GetTicketsByStatusQuery : IRequest<List<TicketDto>>
{
    public TicketStatus Status { get; set; }

    public GetTicketsByStatusQuery(TicketStatus status)
    {
        Status = status;
    }
}

public class GetTicketsByStatusQueryHandler : IRequestHandler<GetTicketsByStatusQuery, List<TicketDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTicketsByStatusQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TicketDto>> Handle(GetTicketsByStatusQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _unitOfWork.Tickets.GetByStatusAsync(request.Status);
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
