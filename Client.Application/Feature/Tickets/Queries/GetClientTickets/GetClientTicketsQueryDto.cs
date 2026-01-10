using TicketingSystem.Domain.Enums;

public class GetClientTicketsQueryDto
{
    public Guid ClientId { get; set; }
    public TicketStatus? Status { get; set; }
    public TicketPriority? Priority { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}