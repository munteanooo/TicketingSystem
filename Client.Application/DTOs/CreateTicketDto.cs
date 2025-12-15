namespace TicketingSystem.Application.DTOs
{
    public class CreateTicketDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Priority { get; set; }
    }
}