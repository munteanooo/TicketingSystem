using MediatR;
using TicketingSystem.Application.Contracts.Exceptions;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Application.Tickets.Commands.AddMessage
{
    public class AddMessageCommandHandler : IRequestHandler<AddMessageCommand, AddMessageCommandResponseDto>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;

        public AddMessageCommandHandler(
            ITicketRepository ticketRepository,
            IUserRepository userRepository,
            ICurrentUser currentUser)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _currentUser = currentUser;
        }

        public async Task<AddMessageCommandResponseDto> Handle(AddMessageCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(request.CommandDto.TicketId, cancellationToken);
            if (ticket == null)
                throw NotFoundException.Create(nameof(Ticket), request.CommandDto.TicketId);

            // Verify user is either the ticket owner or the assigned technician
            if (ticket.ClientId.ToString() != _currentUser.UserId && ticket.AssignedTechnicianId?.ToString() != _currentUser.UserId)
                throw ForbiddenException.InvalidOwner(nameof(Ticket));

            var author = await _userRepository.GetByIdAsync(Guid.Parse(_currentUser.UserId!), cancellationToken);
            if (author == null)
                throw NotFoundException.Create(nameof(User), _currentUser.UserId!);

            var message = new TicketMessage
            {
                Id = Guid.NewGuid(),
                TicketId = ticket.Id,
                Content = request.CommandDto.Content,
                AuthorId = author.Id,
                CreatedAt = DateTime.UtcNow,
                Ticket = ticket,
                Author = author
            };

            ticket.Messages.Add(message);
            await _ticketRepository.UpdateAsync(ticket, cancellationToken);

            return MapToDto(message);
        }

        private AddMessageCommandResponseDto MapToDto(TicketMessage message)
        {
            return new AddMessageCommandResponseDto
            {
                Id = message.Id,
                TicketId = message.TicketId,
                Content = message.Content,
                AuthorId = message.AuthorId,
                CreatedAt = message.CreatedAt
            };
        }
    }
}