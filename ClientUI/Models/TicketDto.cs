namespace ClientUI.Models
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Open";
        public string Priority { get; set; } = "Normal";
        public string Category { get; set; } = "General";
        public DateTime CreatedAt { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedByUserEmail { get; set; } = string.Empty;
    }
}
