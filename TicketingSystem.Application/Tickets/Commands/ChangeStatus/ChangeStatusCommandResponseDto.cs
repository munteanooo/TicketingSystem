namespace TicketingSystem.Application.Tickets.Commands.ChangeStatus
{
    public class ChangeStatusCommandResponseDto
    {
        public required Guid Id { get; set; }
        public required string TicketNumber { get; set; }
        public required string Title { get; set; }
        public required string Status { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}