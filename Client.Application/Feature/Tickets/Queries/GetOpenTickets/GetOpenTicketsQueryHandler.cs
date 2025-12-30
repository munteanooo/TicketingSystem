using Client.Application.Contracts.Persistence;
using MediatR;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;

namespace Client.Application.Feature.Tickets.Queries.GetOpenTickets
{
    public class GetOpenTicketsQueryHandler
        : IRequestHandler<GetOpenTicketsQuery, List<GetOpenTicketsQueryResponseDto>>
    {
        private readonly ITicketRepository _ticketRepository;

        public GetOpenTicketsQueryHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<List<GetOpenTicketsQueryResponseDto>> Handle(
            GetOpenTicketsQuery request,
            CancellationToken cancellationToken)
        {
            var tickets = (await _ticketRepository.GetAllAsync())
                .Where(t => t.Status == TicketStatus.Open || t.Status == TicketStatus.Reopened)
                .ToList();

            if (!string.IsNullOrEmpty(request.Filters.Priority))
                tickets = tickets.Where(t => t.Priority.ToString() == request.Filters.Priority).ToList();

            return tickets.Select(MapToDto).ToList();
        }

        private static GetOpenTicketsQueryResponseDto MapToDto(Ticket ticket)
        {
            return new GetOpenTicketsQueryResponseDto
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
