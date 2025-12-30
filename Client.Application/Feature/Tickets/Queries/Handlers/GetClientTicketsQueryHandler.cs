using Client.Application.Contracts.Persistence;
using Client.Application.Feature.Tickets.Commands.Ticket;
using MediatR;
using TicketingSystem.Domain.Entities;

namespace Client.Application.Feature.Tickets.Queries.Handlers
{
    public class GetClientTicketsQueryHandler : IRequestHandler<GetClientTicketsQuery, List<TicketCommandResponseDto>>
    {
        private readonly ITicketRepository _ticketRepository;

        public GetClientTicketsQueryHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<List<TicketCommandResponseDto>> Handle(GetClientTicketsQuery request, CancellationToken cancellationToken)
        {
            // Folosim metoda din repository pentru a obține toate tichetele clientului
            var tickets = await _ticketRepository.GetByUserAsync(request.ClientId);

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
