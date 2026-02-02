namespace TicketingSystem.Application.Tickets.Queries.GetClientTickets
{
    public class GetClientTicketsQueryResponseDto
    {
        public required Guid Id { get; set; }
        public required string TicketNumber { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
        public required string Priority { get; set; }
        public required string Status { get; set; }
        public required Guid ClientId { get; set; }
        public Guid? AssignedTechnicianId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }
}