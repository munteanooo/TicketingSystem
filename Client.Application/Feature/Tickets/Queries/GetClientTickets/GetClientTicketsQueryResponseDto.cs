namespace Client.Application.Feature.Tickets.Queries.GetClientTickets
{
    public class GetClientTicketsQueryResponseDto
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public Guid ClientId { get; set; }
        public string? ClientName { get; set; }
        public Guid? AssignedToAgentId { get; set; }
        public string? AssignedToAgentName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
