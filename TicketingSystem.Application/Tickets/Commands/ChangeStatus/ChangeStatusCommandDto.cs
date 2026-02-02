namespace TicketingSystem.Application.Tickets.Commands.ChangeStatus
{
    public class ChangeStatusCommandDto
    {
        public required Guid TicketId { get; set; }
        public required string Status { get; set; }
    }
}