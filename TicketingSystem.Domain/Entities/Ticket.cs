using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Domain.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; }

        public int CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; }

        public int? AssignedToId { get; set; }
        public ApplicationUser AssignedTo { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();

        public Ticket(string title, string description, TicketPriority priority, int createdById)
        {
            Title = title;
            Description = description;
            Priority = priority;
            CreatedById = createdById;
            Status = TicketStatus.Open;
            CreatedAt = DateTime.UtcNow;
        }

        public void Assign(int userId)
        {
            AssignedToId = userId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeStatus(TicketStatus newStatus)
        {
            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
            if (newStatus == TicketStatus.Resolved)
                ResolvedAt = DateTime.UtcNow;
        }

        public void AddMessage(TicketMessage message)
        {
            Messages.Add(message);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}