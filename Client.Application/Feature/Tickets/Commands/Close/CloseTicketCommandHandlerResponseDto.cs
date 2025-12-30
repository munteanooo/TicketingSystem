namespace Client.Application.Commands.Handlers
{
    public class CloseTicketCommandHandlerResponseDto
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public Guid ClientId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}