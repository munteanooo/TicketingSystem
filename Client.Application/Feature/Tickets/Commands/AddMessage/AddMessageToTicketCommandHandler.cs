using Client.Application.Contracts.Persistence;
using MediatR;
using TicketingSystem.Domain.Entities;

namespace Client.Application.Feature.Tickets.Commands.AddMessage
{
    public class AddMessageToTicketCommandHandler : IRequestHandler<AddMessageToTicketCommand, AddMessageToTicketCommandResponseDto>
    {
        private readonly ITicketRepository _ticketRepository;

        public AddMessageToTicketCommandHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<AddMessageToTicketCommandResponseDto> Handle(AddMessageToTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);
            if (ticket == null)
                throw new Exception("Ticket not found");

            var message = new TicketMessage
            {
                Id = Guid.NewGuid(),
                TicketId = request.TicketId,
                AuthorId = request.AuthorId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow
            };

            ticket.TicketMessages.Add(message);
            ticket.UpdatedAt = DateTime.UtcNow;

            await _ticketRepository.UpdateAsync(ticket);

            return new AddMessageToTicketCommandResponseDto
            {
                Id = message.Id,
                TicketId = message.TicketId,
                AuthorId = message.AuthorId,
                AuthorName = request.AuthorName,
                Content = message.Content,
                CreatedAt = message.CreatedAt
            };
        }
    }
}
