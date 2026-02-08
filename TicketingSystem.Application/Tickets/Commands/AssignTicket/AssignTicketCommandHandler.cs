using MediatR;
using TicketingSystem.Application.Contracts.Exceptions;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Application.Tickets.Commands.AssignTicket
{
    public class AssignTicketCommandHandler : IRequestHandler<AssignTicketCommand, AssignTicketCommandResponseDto>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;

        public AssignTicketCommandHandler(
            ITicketRepository ticketRepository,
            IUserRepository userRepository,
            ICurrentUser currentUser)
        {
            _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        }

        public async Task<AssignTicketCommandResponseDto> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
        {
            // 1. Autorizare: Doar Admin sau Staff Tehnic
            if (!_currentUser.IsAdmin && !_currentUser.IsTechnician)
            {
                throw ForbiddenException.Create("assign", nameof(Ticket));
            }

            // 2. Verifică existența Tichetului
            var ticket = await _ticketRepository.GetByIdForAdminAsync(request.CommandDto.TicketId, cancellationToken);
            if (ticket == null)
                throw NotFoundException.Create(nameof(Ticket), request.CommandDto.TicketId);

            // 3. Verifică existența Tehnicianului
            var technician = await _userRepository.GetByIdAsync(request.CommandDto.TechnicianId, cancellationToken);
            if (technician == null)
                throw NotFoundException.Create(nameof(User), request.CommandDto.TechnicianId);

            // 4. Logica de Business
            ticket.AssignedTechnicianId = technician.Id;
            ticket.AssignedAt = DateTime.UtcNow;

            // IMPORTANT: set status to InProgress when assigned and update timestamps
            ticket.Status = TicketStatus.InProgress;
            ticket.UpdatedAt = DateTime.UtcNow;

            // 5. Stage update — UnitOfWorkBehavior va persista
            await _ticketRepository.UpdateAsync(ticket, cancellationToken);

            // 6. Răspuns DTO
            return new AssignTicketCommandResponseDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber ?? "N/A",
                Title = ticket.Title,
                Status = ticket.Status.ToString(),
                AssignedTechnicianId = ticket.AssignedTechnicianId,
                AssignedAt = ticket.AssignedAt
            };
        }
    }
}