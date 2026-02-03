namespace TicketingSystem.Application.Tickets.Commands.CreateTicket
{
    public class CreateTicketCommandDto
    {
        public Guid ClientId { get; set; } 
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
        public required string Priority { get; set; }
    }
}