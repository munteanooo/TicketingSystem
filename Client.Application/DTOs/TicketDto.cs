using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Application.DTOs
{
    public class TicketDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; }
        public int CreatedById { get; set; }
        public int? AssignedToId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}