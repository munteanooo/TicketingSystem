namespace TicketingSystem.Application.Tickets.Commands.AddMessage
{
    public class AddMessageCommandDto
    {
        public required Guid TicketId { get; set; }
        public required string Content { get; set; }
    }
}