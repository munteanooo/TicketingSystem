using MediatR;
using TicketingSystem.Application.Contracts.Exceptions;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Application.Tickets.Commands.ChangeStatus
{
    public class ChangeStatusCommandHandler : IRequestHandler<ChangeStatusCommand, ChangeStatusCommandResponseDto>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ICurrentUser _currentUser;

        public ChangeStatusCommandHandler(
            ITicketRepository ticketRepository,
            ICurrentUser currentUser)
        {
            _ticketRepository = ticketRepository;
            _currentUser = currentUser;
        }

        public async Task<ChangeStatusCommandResponseDto> Handle(ChangeStatusCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(request.CommandDto.TicketId, cancellationToken);
            if (ticket == null)
                throw NotFoundException.Create(nameof(Ticket), request.CommandDto.TicketId);

            // Verify user has permission to change status
            if (ticket.AssignedTechnicianId?.ToString() != _currentUser.UserId && !_currentUser.IsAdmin)
                throw ForbiddenException.Create("change status", nameof(Ticket));

            // Validate status enum
            if (!Enum.TryParse<TicketStatus>(request.CommandDto.Status, true, out var newStatus))
                throw new ArgumentException($"Invalid status: {request.CommandDto.Status}");

            ticket.Status = newStatus;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _ticketRepository.UpdateAsync(ticket, cancellationToken);

            return MapToDto(ticket);
        }

        private ChangeStatusCommandResponseDto MapToDto(Ticket ticket)
        {
            return new ChangeStatusCommandResponseDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                Title = ticket.Title,
                Status = ticket.Status.ToString(),
                UpdatedAt = ticket.UpdatedAt
            };
        }
    }
}