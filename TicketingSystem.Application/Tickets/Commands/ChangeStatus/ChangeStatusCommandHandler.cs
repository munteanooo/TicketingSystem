using MediatR;
using TicketingSystem.Application.Contracts.Exceptions;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Application.Tickets.Commands.ChangeStatus
{
    public class ChangeStatusCommandHandler : IRequestHandler<ChangeStatusCommand, ChangeStatusCommandResponseDto>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ICurrentUser _currentUser;

        public ChangeStatusCommandHandler(ITicketRepository ticketRepository, ICurrentUser currentUser)
        {
            _ticketRepository = ticketRepository;
            _currentUser = currentUser;
        }

        public async Task<ChangeStatusCommandResponseDto> Handle(ChangeStatusCommand request, CancellationToken cancellationToken)
        {
            // 1. Recuperare tichet
            var ticket = await _ticketRepository.GetByIdAsync(request.CommandDto.TicketId, cancellationToken);

            if (ticket == null)
                throw NotFoundException.Create(nameof(Ticket), request.CommandDto.TicketId);

            // 2. Securitate: Doar tehnicianul alocat sau un Admin pot schimba statusul
            if (ticket.AssignedTechnicianId?.ToString() != _currentUser.UserId && !_currentUser.IsAdmin)
                throw ForbiddenException.Create("change status", nameof(Ticket));

            // 3. Validare format Status (Conversie din String în Enum)
            if (!Enum.TryParse<TicketStatus>(request.CommandDto.Status, true, out var newStatus))
                throw new ValidationException($"Statusul '{request.CommandDto.Status}' nu este valid.");

            // 4. VALIDARE BUSINESS: Blocăm "In Progress" dacă nu există un tehnician alocat
            if (newStatus == TicketStatus.InProgress && ticket.AssignedTechnicianId == null)
            {
                throw new ValidationException("Tichetul trebuie să fie alocat unui tehnician înainte de a fi trecut în 'In Progress'.");
            }

            // 5. Aplicare modificări
            ticket.Status = newStatus;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _ticketRepository.UpdateAsync(ticket, cancellationToken);

            // 6. Returnare răspuns
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