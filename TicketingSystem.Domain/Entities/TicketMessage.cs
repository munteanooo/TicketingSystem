namespace TicketingSystem.Domain.Entities
{
    public class TicketMessage
    {
        public Guid Id { get; set; }

        public Guid TicketId { get; set; }
        public Guid AuthorId { get; set; }

        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public required Ticket Ticket { get; set; }
        public required User Author { get; set; }
    }
}