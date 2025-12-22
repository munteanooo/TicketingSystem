namespace TicketingSystem.Domain.Entities
{
    public class TicketMessage
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public Guid AuthorId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual DomainUser Author { get; set; }
    }
}