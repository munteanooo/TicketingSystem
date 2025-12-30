using Client.Application.Contracts.Persistence;
using Client.Application.Feature.Tickets.Commands.Ticket;
using MediatR;
using TicketingSystem.Domain.Enums;

namespace Client.Application.Feature.Tickets.Commands.Assign
{
    public class AssignTicketCommandHandler : IRequestHandler<AssignTicketCommand, TicketCommandResponseDto>
    {
        private readonly ITicketRepository _ticketRepository;

        public AssignTicketCommandHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<TicketCommandResponseDto> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
        {
            // Obținem ticket-ul real din repository
            var ticket = await _ticketRepository.GetByIdAsync(request.TicketId);
            if (ticket == null)
                throw new Exception("Ticket not found");

            // Actualizăm proprietățile
            ticket.AssignedToAgentId = request.AgentId;
            ticket.Status = TicketStatus.InProgress;
            ticket.UpdatedAt = DateTime.UtcNow;

            // Salvăm modificările
            await _ticketRepository.UpdateAsync(ticket);

            // Mapăm domeniul Ticket → TicketCommandResponseDto
            return MapToDto(ticket);
        }

        // Schimbăm parametrul să fie entitatea reală Ticket
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
                AssignedToAgentId = ticket.AssignedToAgentId,
                UpdatedAt = ticket.UpdatedAt ?? DateTime.MinValue
            };
        }
    }
}
