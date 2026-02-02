namespace TicketingSystem.Application.Tickets.Commands.AssignTicket
{
    public class AssignTicketCommandResponseDto
    {
        public required Guid Id { get; set; }
        public required string TicketNumber { get; set; }
        public required string Title { get; set; }
        public required string Status { get; set; }
        public Guid? AssignedTechnicianId { get; set; }
        public DateTime? AssignedAt { get; set; }
    }
}