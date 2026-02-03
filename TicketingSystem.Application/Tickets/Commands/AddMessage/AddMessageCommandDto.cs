namespace TicketingSystem.Application.Tickets.Commands.AddMessage
{
    public class AddMessageCommandDto
    {
        public Guid TicketId { get; set; }
        public Guid AuthorId { get; set; }
        public required string Content { get; set; }
    }
}