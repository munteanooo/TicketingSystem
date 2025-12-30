namespace Client.Application.Feature.Tickets.Commands.AddMessage
{
    public class AddMessageToTicketCommandDto
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public Guid AuthorId { get; set; }
        public required string AuthorName { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
