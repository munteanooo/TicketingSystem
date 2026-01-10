using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Domain.Entities
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; }
        public string Category { get; set; } = string.Empty;

        public Guid ClientId { get; set; }
        public Guid? AssignedToAgentId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public required User Client { get; set; }
        public User? AssignedToAgent { get; set; }

        public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
    }
}
