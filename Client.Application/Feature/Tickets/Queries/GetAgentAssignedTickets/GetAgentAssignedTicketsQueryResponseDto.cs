namespace Client.Application.Feature.Tickets.Queries.GetAgentAssignedTickets
{
    public class GetAgentAssignedTicketsQueryResponseDto
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public Guid ClientId { get; set; }
        public string? ClientName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid? AssignedToAgentId { get; set; }
    }
}
