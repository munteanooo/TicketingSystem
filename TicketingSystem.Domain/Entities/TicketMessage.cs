namespace TicketingSystem.Domain.Entities
{
    public class TicketMessage
    {
        public required Guid Id { get; set; }
        public required Guid TicketId { get; set; }
        public required string Content { get; set; }
        public required Guid AuthorId { get; set; }
        public required DateTime CreatedAt { get; set; }

        public required Ticket Ticket { get; set; }
        public required User Author { get; set; }
    }
}