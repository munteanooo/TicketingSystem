using TicketingSystem.Domain.Entities;

public class TicketMessage
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public Ticket Ticket { get; set; }

    public int AuthorId { get; set; }
    public ApplicationUser Author { get; set; }

    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }

    public TicketMessage(string content, int authorId)
    {
        Content = content;
        AuthorId = authorId;
        CreatedAt = DateTime.UtcNow;
    }
}