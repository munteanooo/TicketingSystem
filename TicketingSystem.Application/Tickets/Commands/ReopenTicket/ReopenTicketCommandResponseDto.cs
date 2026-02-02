namespace TicketingSystem.Application.Tickets.Commands.ReopenTicket
{
    public class ReopenTicketCommandResponseDto
    {
        public required Guid Id { get; set; }
        public required string TicketNumber { get; set; }
        public required string Title { get; set; }
        public required string Status { get; set; }
        public string? ReopenReason { get; set; }
        public DateTime? ReopenedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}