using MediatR;
using TicketingSystem.Application.Contracts.Exceptions;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Application.Tickets.Queries.GetTicketMessages
{
    public class GetTicketMessagesQueryHandler : IRequestHandler<GetTicketMessagesQuery, IEnumerable<GetTicketMessagesQueryResponseDto>>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ICurrentUser _currentUser;

        public GetTicketMessagesQueryHandler(
            ITicketRepository ticketRepository,
            ICurrentUser currentUser)
        {
            _ticketRepository = ticketRepository;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<GetTicketMessagesQueryResponseDto>> Handle(GetTicketMessagesQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);
            if (ticket == null)
                throw NotFoundException.Create(nameof(Ticket), request.TicketId);

            // Verify user has access: either ticket owner, assigned technician, or admin
            if (ticket.ClientId.ToString() != _currentUser.UserId
                && ticket.AssignedTechnicianId?.ToString() != _currentUser.UserId
                && !_currentUser.IsAdmin)
                throw ForbiddenException.InvalidOwner(nameof(Ticket));

            // Return messages ordered by creation date (oldest first)
            var messages = ticket.Messages?
                .OrderBy(m => m.CreatedAt)
                .Select(MapToDto)
                .ToList() ?? new List<GetTicketMessagesQueryResponseDto>();

            return messages;
        }

        private GetTicketMessagesQueryResponseDto MapToDto(TicketMessage message)
        {
            return new GetTicketMessagesQueryResponseDto
            {
                Id = message.Id,
                TicketId = message.TicketId,
                Content = message.Content,
                AuthorId = message.AuthorId,
                AuthorName = message.Author?.FullName,
                AuthorEmail = message.Author?.Email,
                CreatedAt = message.CreatedAt
            };
        }
    }
}