namespace TicketingSystem.Domain.Entities
{
    public class TicketMessage
    {
        public required Guid Id { get; set; }
        public required Guid TicketId { get; set; }
        public required string Content { get; set; }
        public required Guid AuthorId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public Ticket Ticket { get; set; } = null!;
        public User Author { get; set; } = null!;
    }
}