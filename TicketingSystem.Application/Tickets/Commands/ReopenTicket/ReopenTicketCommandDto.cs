namespace TicketingSystem.Application.Tickets.Commands.ReopenTicket
{
    public class ReopenTicketCommandDto
    {
        public required Guid TicketId { get; set; }
        public string? Reason { get; set; }
    }
}