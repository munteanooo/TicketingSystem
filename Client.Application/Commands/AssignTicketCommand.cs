using MediatR;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Interfaces;

public class AssignTicketCommand : IRequest<TicketDto>
{
    public int TicketId { get; set; }
    public int AssignToUserId { get; set; }
}

public class AssignTicketCommandHandler : IRequestHandler<AssignTicketCommand, TicketDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public AssignTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketDto> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.TicketId);
        if (ticket == null)
            throw new Exception($"Ticket {request.TicketId} not found");

        ticket.Assign(request.AssignToUserId);

        await _unitOfWork.Tickets.UpdateAsync(ticket);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(ticket);
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
            CreatedAt = ticket.CreatedAt
        };
    }
}