using MediatR;
using TicketingSystem.Application.Contracts.Exceptions;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Application.Tickets.Queries.GetClientTickets
{
    public class GetClientTicketsQueryHandler : IRequestHandler<GetClientTicketsQuery, IEnumerable<GetClientTicketsQueryResponseDto>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;

        public GetClientTicketsQueryHandler(
            ITicketRepository ticketRepository,
            IUserRepository userRepository,
            ICurrentUser currentUser)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<GetClientTicketsQueryResponseDto>> Handle(GetClientTicketsQuery request, CancellationToken cancellationToken)
        {
            // Verify the client exists
            var client = await _userRepository.GetByIdAsync(request.ClientId, cancellationToken);
            if (client == null)
                throw NotFoundException.Create(nameof(User), request.ClientId);

            // Verify current user is either the client or an admin
            if (request.ClientId.ToString() != _currentUser.UserId && !_currentUser.IsAdmin)
                throw ForbiddenException.Create("view", "client tickets");

            var tickets = await _ticketRepository.GetByClientIdAsync(request.ClientId, cancellationToken);

            return tickets.Select(MapToDto).ToList();
        }

        private GetClientTicketsQueryResponseDto MapToDto(Ticket ticket)
        {
            return new GetClientTicketsQueryResponseDto
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
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                ClosedAt = ticket.ClosedAt
            };
        }
    }
}