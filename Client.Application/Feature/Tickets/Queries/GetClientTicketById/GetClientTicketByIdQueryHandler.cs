using Client.Application.Contracts.Persistence;
using MediatR;

namespace Client.Application.Feature.Tickets.Queries.GetClientTicketById
{
    public class GetClientTicketByIdQueryHandler
        : IRequestHandler<GetClientTicketByIdQuery, GetClientTicketByIdResponseDto>
    {
        private readonly ITicketRepository _ticketRepository;

        public GetClientTicketByIdQueryHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<GetClientTicketByIdResponseDto> Handle(
            GetClientTicketByIdQuery request,
            CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(request.QueryDto.TicketId);

            if (ticket == null || ticket.ClientId != request.QueryDto.ClientId)
                throw new KeyNotFoundException("Ticket not found or does not belong to client.");

            return new GetClientTicketByIdResponseDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status.ToString(),
                Priority = ticket.Priority.ToString(),
                CreatedAt = ticket.CreatedAt,
                Messages = ticket.Messages?.Select(m => new TicketMessageDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                    Sender = m.AuthorId == ticket.ClientId ? "Client" : "TechSupport"
                }).ToList()
            };
        }
    }
}
