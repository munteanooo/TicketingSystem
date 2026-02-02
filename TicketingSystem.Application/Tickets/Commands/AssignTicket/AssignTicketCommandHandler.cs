using MediatR;
using TicketingSystem.Application.Contracts.Exceptions;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;

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
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _currentUser = currentUser;
        }

        public async Task<AssignTicketCommandResponseDto> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAdmin && !_currentUser.IsTechnician)
                throw ForbiddenException.Create("assign", nameof(Ticket));

            var ticket = await _ticketRepository.GetByIdAsync(request.CommandDto.TicketId, cancellationToken);
            if (ticket == null)
                throw NotFoundException.Create(nameof(Ticket), request.CommandDto.TicketId);

            var technician = await _userRepository.GetByIdAsync(request.CommandDto.TechnicianId, cancellationToken);
            if (technician == null)
                throw NotFoundException.Create(nameof(User), request.CommandDto.TechnicianId);

            ticket.AssignedTechnicianId = technician.Id;
            ticket.AssignedTechnician = technician;
            ticket.AssignedAt = DateTime.UtcNow;

            await _ticketRepository.UpdateAsync(ticket, cancellationToken);

            return MapToDto(ticket);
        }

        private AssignTicketCommandResponseDto MapToDto(Ticket ticket)
        {
            return new AssignTicketCommandResponseDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                Title = ticket.Title,
                Status = ticket.Status.ToString(),
                AssignedTechnicianId = ticket.AssignedTechnicianId,
                AssignedAt = ticket.AssignedAt
            };
        }
    }
}