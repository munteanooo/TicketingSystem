using Client.Application.Feature.Tickets.Commands.Ticket;

namespace Client.Application.Feature.Tickets.Queries.GetTicketMessages
{
    public class GetTicketMessagesQueryResponseDto
    {
        public Guid TicketId { get; set; }
        public List<TicketMessageDto> Messages { get; set; } = new();
    }
}
