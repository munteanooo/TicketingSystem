namespace TicketingSystem.Application.Tickets.Queries.GetTicketDetails
{
    public class GetTicketDetailsQueryResponseDto
    {
        public required Guid Id { get; set; }
        public required string TicketNumber { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
        public required string Priority { get; set; }
        public required string Status { get; set; }
        public required Guid ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? ClientEmail { get; set; }
        public Guid? AssignedTechnicianId { get; set; }
        public string? AssignedTechnicianName { get; set; }
        public string? ResolutionNote { get; set; }
        public string? ReopenReason { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public DateTime? ReopenedAt { get; set; }
        public int MessageCount { get; set; }

        public List<TicketMessageDto> Messages { get; set; } = new();
    }

    public class TicketMessageDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}