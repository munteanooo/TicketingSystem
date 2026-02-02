namespace TicketingSystem.Application.Tickets.Queries.GetTicketMessages
{
    public class GetTicketMessagesQueryResponseDto
    {
        public required Guid Id { get; set; }
        public required Guid TicketId { get; set; }
        public required string Content { get; set; }
        public required Guid AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorEmail { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}