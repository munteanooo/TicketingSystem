using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string IdentityUserId { get; set; } 
        public string Name { get; set; }
        public UserRole Role { get; set; }
        public string Email { get; set; }
        public ICollection<Ticket> CreatedTickets { get; set; }
        public ICollection<Ticket> AssignedTickets { get; set; }
        public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
    }
}