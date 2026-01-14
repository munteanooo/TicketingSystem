using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public Guid? IdentityUserId { get; set; }

        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }

        public UserRole Role { get; set; } = UserRole.Client;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Ticket> SubmittedTickets { get; set; } = new List<Ticket>();
        public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
        public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
    }
}