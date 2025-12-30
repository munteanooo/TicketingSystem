using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            // Preluăm ticket-ul cu toate detaliile
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(request.Filters.TicketId, cancellationToken);

            if (ticket == null)
                throw new Exception("Ticket not found");

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
