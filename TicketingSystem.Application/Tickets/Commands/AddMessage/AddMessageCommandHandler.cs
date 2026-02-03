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
            // 1. Verificăm dacă ticketul există
            var ticket = await _ticketRepository.GetByIdAsync(request.CommandDto.TicketId, cancellationToken);
            if (ticket == null)
                throw NotFoundException.Create(nameof(Ticket), request.CommandDto.TicketId);

            // 2. Securitate: Doar posesorul sau tehnicianul pot scrie
            var currentUserId = Guid.Parse(_currentUser.UserId!);
            if (ticket.ClientId != currentUserId && ticket.AssignedTechnicianId != currentUserId)
                throw ForbiddenException.InvalidOwner(nameof(Ticket));

            // 3. Creăm entitatea de mesaj
            var message = new TicketMessage
            {
                Id = Guid.NewGuid(),
                TicketId = ticket.Id,
                AuthorId = currentUserId,
                Content = request.CommandDto.Content,
                CreatedAt = DateTime.UtcNow
            };

            // 4. Salvare directă (EVITĂM ticket.Messages.Add pentru a preveni Concurrency Error)
            await _ticketRepository.AddMessageAsync(message, cancellationToken);
            await _ticketRepository.SaveChangesAsync(cancellationToken);

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