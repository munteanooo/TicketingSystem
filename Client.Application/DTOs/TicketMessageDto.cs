namespace TicketingSystem.Application.DTOs
{   
    public class TicketMessageDto
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}