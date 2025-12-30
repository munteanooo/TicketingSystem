using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;

public class User
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

    public ICollection<Ticket> SubmittedTickets { get; set; }
    public ICollection<Ticket> AssignedTickets { get; set; }
    public ICollection<TicketMessage> Messages { get; set; }
}
