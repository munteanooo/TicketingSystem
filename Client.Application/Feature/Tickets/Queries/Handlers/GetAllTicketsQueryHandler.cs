using Client.Application.Contracts.Persistence;
using Client.Application.Feature.Tickets.Commands.Ticket;
using MediatR;
using TicketingSystem.Domain.Entities;

namespace Client.Application.Feature.Tickets.Queries.Handlers
{
    public class GetAllTicketsQueryHandler
        : IRequestHandler<GetAllTicketsQuery, List<TicketCommandResponseDto>>
    {
        private readonly ITicketRepository _ticketRepository;

        public GetAllTicketsQueryHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<List<TicketCommandResponseDto>> Handle(
            GetAllTicketsQuery request,
            CancellationToken cancellationToken)
        {
            // Preia toate tichetele cu detalii (Client și Agent)
            var tickets = await _ticketRepository
                .GetAllWithDetailsAsync(cancellationToken);

            // Transformă în DTO
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
