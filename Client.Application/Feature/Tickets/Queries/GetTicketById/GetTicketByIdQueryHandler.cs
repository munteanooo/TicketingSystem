using Client.Application.Contracts.Persistence;
using Client.Application.Feature.Tickets.Commands.Ticket;
using MediatR;
using TicketingSystem.Domain.Entities;

namespace Client.Application.Feature.Tickets.Queries.GetTicketById
{
    public class GetTicketByIdQueryHandler
        : IRequestHandler<GetTicketByIdQuery, GetTicketByIdQueryResponseDto>
    {
        private readonly ITicketRepository _ticketRepository;

        public GetTicketByIdQueryHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<GetTicketByIdQueryResponseDto> Handle(
            GetTicketByIdQuery request,
            CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(request.Filters.TicketId, cancellationToken);

            if (ticket == null)
                throw new Exception("Ticket not found");

            return MapToDto(ticket);
        }

        private static GetTicketByIdQueryResponseDto MapToDto(Ticket ticket)
        {
            return new GetTicketByIdQueryResponseDto
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
                ClientEmail = ticket.Client?.Email,
                AssignedToAgentId = ticket.AssignedToAgentId,
                AssignedToAgentName = ticket.AssignedToAgent?.FullName,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt ?? DateTime.MinValue,
                ResolvedAt = ticket.ResolvedAt,
                Messages = ticket.Messages?.Select(m => new TicketMessageDto
                {
                    Id = m.Id,
                    TicketId = m.TicketId,
                    AuthorId = m.AuthorId,
                    AuthorName = m.Author?.FullName,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt
                }).ToList() ?? new List<TicketMessageDto>()
            };
        }
    }
}
