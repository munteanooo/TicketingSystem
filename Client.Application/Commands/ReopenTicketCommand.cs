using MediatR;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.Interfaces;

public class ReopenTicketCommand : IRequest<TicketDto>
{
    public int TicketId { get; set; }
}

public class ReopenTicketCommandHandler : IRequestHandler<ReopenTicketCommand, TicketDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReopenTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketDto> Handle(ReopenTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.TicketId);
        if (ticket == null)
            throw new Exception($"Ticket {request.TicketId} not found");

        ticket.ChangeStatus(TicketStatus.Reopened);

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