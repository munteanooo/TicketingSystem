using MediatR;
using TicketingSystem.Application.Contracts.Exceptions;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Application.Tickets.Queries.GetTechTickets
{
    public class GetTechTicketsQueryHandler : IRequestHandler<GetTechTicketsQuery, IEnumerable<GetTechTicketsQueryResponseDto>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;

        public GetTechTicketsQueryHandler(
            ITicketRepository ticketRepository,
            IUserRepository userRepository,
            ICurrentUser currentUser)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<GetTechTicketsQueryResponseDto>> Handle(GetTechTicketsQuery request, CancellationToken cancellationToken)
        {
            // Verify the technician exists
            var technician = await _userRepository.GetByIdAsync(request.TechnicianId, cancellationToken);
            if (technician == null)
                throw NotFoundException.Create(nameof(User), request.TechnicianId);

            // Verify current user is either the technician or an admin
            if (request.TechnicianId.ToString() != _currentUser.UserId && !_currentUser.IsAdmin)
                throw ForbiddenException.Create("view", "technician tickets");

            var tickets = await _ticketRepository.GetByAssignedTechnicianAsync(request.TechnicianId, cancellationToken);

            return tickets.Select(MapToDto).ToList();
        }

        private GetTechTicketsQueryResponseDto MapToDto(Ticket ticket)
        {
            return new GetTechTicketsQueryResponseDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                Title = ticket.Title,
                Description = ticket.Description,
                Category = ticket.Category,
                Priority = ticket.Priority.ToString(),
                Status = ticket.Status.ToString(),
                ClientId = ticket.ClientId,
                AssignedTechnicianId = ticket.AssignedTechnicianId,
                AssignedAt = ticket.AssignedAt,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                ClosedAt = ticket.ClosedAt
            };
        }
    }
}