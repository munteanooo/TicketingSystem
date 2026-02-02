namespace TicketingSystem.Application.Tickets.Commands.AssignTicket
{
    public class AssignTicketCommandDto
    {
        public required Guid TicketId { get; set; }
        public required Guid TechnicianId { get; set; }
    }
}