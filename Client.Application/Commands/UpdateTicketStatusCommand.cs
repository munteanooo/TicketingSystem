using MediatR;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.Interfaces;

public class UpdateTicketStatusCommand : IRequest<TicketDto>
{
    public int TicketId { get; set; }
    public TicketStatus NewStatus { get; set; }
}

public class UpdateTicketStatusCommandHandler : IRequestHandler<UpdateTicketStatusCommand, TicketDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTicketStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketDto> Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.TicketId);
        if (ticket == null)
            throw new Exception($"Ticket {request.TicketId} not found");

        ticket.ChangeStatus(request.NewStatus);

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
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            ResolvedAt = ticket.ResolvedAt
        };
    }
}
