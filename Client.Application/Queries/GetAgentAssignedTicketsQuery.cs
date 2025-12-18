using MediatR;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Interfaces;

public class GetAgentAssignedTicketsQuery : IRequest<List<TicketDto>>
{
    public int AgentId { get; set; }

    public GetAgentAssignedTicketsQuery(int agentId)
    {
        AgentId = agentId;
    }
}

public class GetAgentAssignedTicketsQueryHandler : IRequestHandler<GetAgentAssignedTicketsQuery, List<TicketDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAgentAssignedTicketsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TicketDto>> Handle(GetAgentAssignedTicketsQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _unitOfWork.Tickets.GetByAssigneeAsync(request.AgentId);
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