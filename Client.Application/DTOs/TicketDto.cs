namespace TicketingSystem.Application.DTOs
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public Guid? AssignedToAgentId { get; set; }
        public string AssignedToAgentName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}