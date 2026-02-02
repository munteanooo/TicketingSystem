namespace TicketingSystem.Domain.Entities
{
    public class Ticket
    {
        public required Guid Id { get; set; }
        public required string TicketNumber { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
        public required TicketPriority Priority { get; set; }
        public required TicketStatus Status { get; set; }

        // Client information
        public required Guid ClientId { get; set; }

        // Technician assignment
        public Guid? AssignedTechnicianId { get; set; }
        public DateTime? AssignedAt { get; set; }

        // Resolution information
        public string? ResolutionNote { get; set; }
        public DateTime? ClosedAt { get; set; }

        // Reopen information
        public string? ReopenReason { get; set; }
        public DateTime? ReopenedAt { get; set; }

        // Timestamps
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public User? Client { get; set; }
        public User? AssignedTechnician { get; set; }
        public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
    }
}