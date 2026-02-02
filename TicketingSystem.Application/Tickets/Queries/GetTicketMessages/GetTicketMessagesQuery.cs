using MediatR;

namespace TicketingSystem.Application.Tickets.Queries.GetTicketMessages
{
    public record GetTicketMessagesQuery(Guid TicketId)
        : IRequest<IEnumerable<GetTicketMessagesQueryResponseDto>>;
}