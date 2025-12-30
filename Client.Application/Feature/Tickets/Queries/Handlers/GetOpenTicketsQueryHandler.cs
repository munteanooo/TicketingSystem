using Client.Application.Contracts.Persistence;
using Client.Application.Feature.Tickets.Commands.Ticket;
using MediatR;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;

namespace Client.Application.Feature.Tickets.Queries.Handlers
{
    public class GetOpenTicketsQueryHandler : IRequestHandler<GetOpenTicketsQuery, List<TicketCommandResponseDto>>
    {
        private readonly ITicketRepository _ticketRepository;

        public GetOpenTicketsQueryHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<List<TicketCommandResponseDto>> Handle(GetOpenTicketsQuery request, CancellationToken cancellationToken)
        {
            // Folosim repository-ul pentru a obține toate tichetele cu status Open sau Reopened
            var tickets = (await _ticketRepository.GetAllAsync())
                .Where(t => t.Status == TicketStatus.Open || t.Status == TicketStatus.Reopened)
                .ToList();

            return tickets.Select(MapToDto).ToList();
        }

        private static TicketCommandResponseDto MapToDto(Ticket ticket)
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
                ClientName = ticket.Client?.FullName,
                AssignedToAgentId = ticket.AssignedToAgentId,
                AssignedToAgentName = ticket.AssignedToAgent?.FullName,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt ?? DateTime.MinValue
            };
        }
    }
}
