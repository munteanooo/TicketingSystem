using MediatR;
using TicketingSystem.Application.Contracts.Exceptions;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Application.Tickets.Commands.ReopenTicket
{
    public class ReopenTicketCommandHandler : IRequestHandler<ReopenTicketCommand, ReopenTicketCommandResponseDto>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ICurrentUser _currentUser;

        public ReopenTicketCommandHandler(
            ITicketRepository ticketRepository,
            ICurrentUser currentUser)
        {
            _ticketRepository = ticketRepository;
            _currentUser = currentUser;
        }

        public async Task<ReopenTicketCommandResponseDto> Handle(ReopenTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(request.CommandDto.TicketId, cancellationToken);
            if (ticket == null)
                throw NotFoundException.Create(nameof(Ticket), request.CommandDto.TicketId);

            // Only the ticket owner or admin can reopen
            if (ticket.ClientId.ToString() != _currentUser.UserId && !_currentUser.IsAdmin)
                throw ForbiddenException.Create("reopen", nameof(Ticket));

            // Can only reopen closed tickets
            if (ticket.Status != TicketStatus.Closed)
                throw new InvalidOperationException("Only closed tickets can be reopened");

            ticket.Status = TicketStatus.Open;
            ticket.ReopenReason = request.CommandDto.Reason;
            ticket.ReopenedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;
            ticket.ClosedAt = null; // Reset closed date

            await _ticketRepository.UpdateAsync(ticket, cancellationToken);

            return MapToDto(ticket);
        }

        private ReopenTicketCommandResponseDto MapToDto(Ticket ticket)
        {
            return new ReopenTicketCommandResponseDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                Title = ticket.Title,
                Status = ticket.Status.ToString(),
                ReopenReason = ticket.ReopenReason,
                ReopenedAt = ticket.ReopenedAt,
                UpdatedAt = ticket.UpdatedAt
            };
        }
    }
}