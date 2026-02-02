namespace TicketingSystem.Application.Tickets.Commands.CloseTicket
{
    public class CloseTicketCommandDto
    {
        public required Guid TicketId { get; set; }
        public string? ResolutionNote { get; set; }
    }
}