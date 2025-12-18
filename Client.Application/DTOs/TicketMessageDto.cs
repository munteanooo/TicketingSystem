namespace TicketingSystem.Application.DTOs
{
    public class TicketMessageDto
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int AuthorId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}