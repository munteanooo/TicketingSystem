using Client.Application.Contracts.Persistence;
using Client.Application.Feature.Tickets.Commands.Ticket;
using MediatR;

namespace Client.Application.Feature.Tickets.Queries.GetTicketMessages
{
    public class GetTicketMessagesQueryHandler
        : IRequestHandler<GetTicketMessagesQuery, GetTicketMessagesQueryResponseDto>
    {
        private readonly ITicketRepository _ticketRepository;

        public GetTicketMessagesQueryHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<GetTicketMessagesQueryResponseDto> Handle(
    GetTicketMessagesQuery request,
    CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(request.TicketId, cancellationToken);

            if (ticket == null)
                throw new Exception("Ticket not found");

            // verificăm accesul
            if (ticket.ClientId != request.UserId && !request.IsTechSupport)
                throw new UnauthorizedAccessException("Nu ai dreptul să vezi acest ticket");

            var response = new GetTicketMessagesQueryResponseDto
            {
                TicketId = ticket.Id,
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

            return response;
        }
    }
}