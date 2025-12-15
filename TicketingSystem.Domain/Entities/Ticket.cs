using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Domain.Entities
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; }
        public string Category { get; set; } 

        public Guid ClientId { get; set; }
        public Guid? AssignedToAgentId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        
        public virtual DomainUser Client { get; set; }
        public virtual DomainUser AssignedToAgent { get; set; }
        public virtual ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
    }
}