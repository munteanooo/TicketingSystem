using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Domain.Entities
{
    public class DomainUser
    {
        public Guid Id { get; set; }
        public Guid? IdentityUserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual ICollection<Ticket> SubmittedTickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
        public virtual ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
    }
}
