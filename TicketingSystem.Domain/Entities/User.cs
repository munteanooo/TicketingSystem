using Microsoft.AspNetCore.Identity; 

namespace TicketingSystem.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public required string FullName { get; set; }
        public required string Role { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>();
        public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
        public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
    }
}