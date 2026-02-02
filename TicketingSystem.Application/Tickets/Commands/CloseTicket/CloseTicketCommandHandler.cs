using MediatR;
using TicketingSystem.Application.Contracts.Exceptions;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Application.Tickets.Commands.CloseTicket
{
    public class CloseTicketCommandHandler : IRequestHandler<CloseTicketCommand, CloseTicketCommandResponseDto>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ICurrentUser _currentUser;

        public CloseTicketCommandHandler(
            ITicketRepository ticketRepository,
            ICurrentUser currentUser)
        {
            _ticketRepository = ticketRepository;
            _currentUser = currentUser;
        }

        public async Task<CloseTicketCommandResponseDto> Handle(CloseTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(request.CommandDto.TicketId, cancellationToken);
            if (ticket == null)
                throw NotFoundException.Create(nameof(Ticket), request.CommandDto.TicketId);

            // Only the assigned technician or admin can close the ticket
            if (ticket.AssignedTechnicianId?.ToString() != _currentUser.UserId && !_currentUser.IsAdmin)
                throw ForbiddenException.Create("close", nameof(Ticket));

            // Cannot close if already closed
            if (ticket.Status == TicketStatus.Closed)
                throw new InvalidOperationException("Ticket is already closed");

            ticket.Status = TicketStatus.Closed;
            ticket.ResolutionNote = request.CommandDto.ResolutionNote;
            ticket.ClosedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _ticketRepository.UpdateAsync(ticket, cancellationToken);

            return MapToDto(ticket);
        }

        private CloseTicketCommandResponseDto MapToDto(Ticket ticket)
        {
            return new CloseTicketCommandResponseDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                Title = ticket.Title,
                Status = ticket.Status.ToString(),
                ResolutionNote = ticket.ResolutionNote,
                ClosedAt = ticket.ClosedAt,
                UpdatedAt = ticket.UpdatedAt
            };
        }
    }
}