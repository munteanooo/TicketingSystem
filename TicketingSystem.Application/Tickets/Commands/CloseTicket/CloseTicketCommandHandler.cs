using MediatR;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Application.Interfaces;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Application.Tickets.Commands.CloseTicket;

public class CloseTicketCommandHandler : IRequestHandler<CloseTicketCommand, CloseTicketCommandResponseDto>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IApplicationDbContext _context; 

    public CloseTicketCommandHandler(ITicketRepository ticketRepository, ICurrentUser currentUser, IApplicationDbContext context)
    {
        _ticketRepository = ticketRepository;
        _currentUser = currentUser;
        _context = context;
    }

    public async Task<CloseTicketCommandResponseDto> Handle(CloseTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdForAdminAsync(request.CommandDto.TicketId, cancellationToken);

        if (ticket == null)
            throw new Exception("Tichetul nu există.");

        bool isAssignedTech = ticket.AssignedTechnicianId?.ToString() == _currentUser.UserId;
        if (!isAssignedTech && !_currentUser.IsAdmin)
            throw new Exception("Nu aveți dreptul să închideți acest tichet.");

        if (ticket.Status == TicketStatus.Closed)
            throw new InvalidOperationException("Tichetul este deja închis.");

        ticket.Status = TicketStatus.Closed;
        ticket.ResolutionNote = request.CommandDto.ResolutionNote ?? "Închis fără notă adițională.";
        ticket.ClosedAt = DateTime.UtcNow;
        ticket.UpdatedAt = DateTime.UtcNow;

        await _ticketRepository.UpdateAsync(ticket, cancellationToken);

        return new CloseTicketCommandResponseDto
        {
            Id = ticket.Id,
            TicketNumber = ticket.TicketNumber ?? "N/A",
            Title = ticket.Title,
            Status = ticket.Status.ToString(),
            ResolutionNote = ticket.ResolutionNote,
            ClosedAt = ticket.ClosedAt,
            UpdatedAt = ticket.UpdatedAt
        };
    }
}