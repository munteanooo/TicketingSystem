namespace TicketingSystem.Application.Tickets.Commands.CloseTicket
{
    public class CloseTicketCommandResponseDto
    {
        public required Guid Id { get; set; }
        public required string TicketNumber { get; set; }
        public required string Title { get; set; }
        public required string Status { get; set; }
        public string? ResolutionNote { get; set; }
        public DateTime? ClosedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}