using MediatR;
using TicketingSystem.Application.Contracts.Exceptions;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Application.Tickets.Commands.AddMessage
{
    public class AddMessageCommandHandler : IRequestHandler<AddMessageCommand, AddMessageCommandResponseDto>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ICurrentUser _currentUser;

        public AddMessageCommandHandler(ITicketRepository ticketRepository, ICurrentUser currentUser)
        {
            _ticketRepository = ticketRepository;
            _currentUser = currentUser;
        }

        public async Task<AddMessageCommandResponseDto> Handle(AddMessageCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdForAdminAsync(request.CommandDto.TicketId, cancellationToken);

            if (ticket == null)
                throw NotFoundException.Create(nameof(Ticket), request.CommandDto.TicketId);

            var currentUserId = Guid.Parse(_currentUser.UserId!);

            // Autorizare: Clientul lui, Tech-ul alocat, sau Admin
            if (ticket.ClientId != currentUserId && ticket.AssignedTechnicianId != currentUserId && !_currentUser.IsAdmin)
                throw ForbiddenException.Create("add message", nameof(Ticket));

            var message = new TicketMessage
            {
                Id = Guid.NewGuid(),
                TicketId = ticket.Id,
                AuthorId = currentUserId,
                Content = request.CommandDto.Content,
                CreatedAt = DateTime.UtcNow
            };

            await _ticketRepository.AddMessageAsync(message, cancellationToken);

            ticket.UpdatedAt = DateTime.UtcNow;
            await _ticketRepository.UpdateAsync(ticket, cancellationToken);

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