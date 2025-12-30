using Client.Application.Contracts.Persistence;
using MediatR;

namespace Client.Application.Feature.Tickets.Commands.Delete
{
    public class DeleteTicketCommandHandler : IRequestHandler<DeleteTicketCommand, bool>
    {
        private readonly ITicketRepository _ticketRepository;

        public DeleteTicketCommandHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<bool> Handle(DeleteTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);

            if (ticket == null)
            {
                return false; // ticket-ul nu există
            }

            await _ticketRepository.DeleteAsync(request.TicketId); // trimitem doar Guid
            return true; // ștergere reușită
        }
    }
}
