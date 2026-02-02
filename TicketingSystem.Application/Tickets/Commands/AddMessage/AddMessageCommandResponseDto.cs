namespace TicketingSystem.Application.Tickets.Commands.AddMessage
{
    public class AddMessageCommandResponseDto
    {
        public required Guid Id { get; set; }
        public required Guid TicketId { get; set; }
        public required string Content { get; set; }
        public required Guid AuthorId { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}