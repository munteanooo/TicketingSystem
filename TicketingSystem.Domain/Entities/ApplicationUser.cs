using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Domain.Entities
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }

        // Relații
        public ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>();
        public ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
        public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();

        // Constructor
        public ApplicationUser(string email, string firstName, string lastName, UserRole role)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
            IsActive = true;
        }

        // Methods
        public string GetFullName() => $"{FirstName} {LastName}";

        public void Deactivate() => IsActive = false;

        public void Activate() => IsActive = true;
    }
}