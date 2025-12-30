using Client.Application.Contracts.Persistence;
using Client.Application.Feature.Tickets.Commands.Ticket;
using MediatR;
using TicketingSystem.Domain.Enums;

namespace Client.Application.Feature.Tickets.Commands.Close
{
    public class CloseTicketCommandHandler : IRequestHandler<CloseTicketCommand, TicketCommandResponseDto>
    {
        private readonly ITicketRepository _ticketRepository;

        public CloseTicketCommandHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<TicketCommandResponseDto> Handle(CloseTicketCommand request, CancellationToken cancellationToken)
        {
            // Folosim metoda repository-ului
            var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);
            if (ticket == null)
                throw new Exception("Ticket not found");

            ticket.Status = TicketStatus.Closed;
            ticket.ResolvedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;

            // Salvăm prin repository
            await _ticketRepository.UpdateAsync(ticket);

            return MapToDto(ticket);
        }

        private TicketCommandResponseDto MapToDto(TicketingSystem.Domain.Entities.Ticket ticket)
        {
            return new TicketCommandResponseDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                Title = ticket.Title,
                Description = ticket.Description,
                Priority = ticket.Priority.ToString(),
                Status = ticket.Status.ToString(),
                Category = ticket.Category,
                ClientId = ticket.ClientId,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt ?? DateTime.MinValue,
                ResolvedAt = ticket.ResolvedAt ?? DateTime.MinValue
            };
        }
    }
}