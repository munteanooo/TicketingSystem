using MediatR;
using TicketingSystem.Application.Contracts.Exceptions;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Application.Tickets.Queries.GetTechTickets;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Application.Tickets.Queries
{
    public class GetUnassignedTicketsQueryHandler : IRequestHandler<GetUnassignedTicketsQuery, IEnumerable<GetTechTicketsQueryResponseDto>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;

        public GetUnassignedTicketsQueryHandler(
            ITicketRepository ticketRepository,
            IUserRepository userRepository,
            ICurrentUser currentUser)
        {
            _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        }

        public async Task<IEnumerable<GetTechTicketsQueryResponseDto>> Handle(GetUnassignedTicketsQuery request, CancellationToken cancellationToken)
        {
            // Only Admin or TechSupport can view unassigned tickets
            var role = _currentUser.UserRole;
            if (!_currentUser.IsAdmin && !string.Equals(role, "TechSupport", StringComparison.OrdinalIgnoreCase))
            {
                throw ForbiddenException.Create("view", "unassigned tickets");
            }

            // Find tickets with no assigned technician
            var tickets = await _ticketRepository.FindAsync(t => t.AssignedTechnicianId == null, cancellationToken);

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
                ClientName = ticket.Client?.FullName ?? "Unknown",
                AssignedTechnicianId = ticket.AssignedTechnicianId,
                AssignedAt = ticket.AssignedAt,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                ClosedAt = ticket.ClosedAt
            };
        }
    }
}