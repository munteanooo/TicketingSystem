using Client.Application.Contracts.Persistence;
using MediatR;
using TicketingSystem.Domain.Entities;

namespace Client.Application.Feature.Tickets.Commands.AddMessage
{
    public class AddMessageToTicketCommandHandler : IRequestHandler<AddMessageToTicketCommand, AddMessageToTicketCommandResponseDto>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;

        public AddMessageToTicketCommandHandler(ITicketRepository ticketRepository, IUserRepository userRepository)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
        }

        public async Task<AddMessageToTicketCommandResponseDto> Handle(AddMessageToTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);
            if (ticket == null)
                throw new Exception("Ticket not found");

            var author = await _userRepository.GetByIdAsync(request.AuthorId);
            if (author == null)
                throw new Exception("Author not found");

            var message = new TicketMessage
            {
                Id = Guid.NewGuid(),
                TicketId = request.TicketId,
                AuthorId = request.AuthorId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                Ticket = ticket,
                Author = author
            };

            ticket.Messages.Add(message);
            ticket.UpdatedAt = DateTime.UtcNow;
            await _ticketRepository.UpdateAsync(ticket);

            return new AddMessageToTicketCommandResponseDto
            {
                Id = message.Id,
                TicketId = message.TicketId,
                AuthorId = message.AuthorId,
                AuthorName = author.FullName,
                Content = message.Content,
                CreatedAt = message.CreatedAt
            };
        }
    }
}