namespace Client.Application.Feature.Tickets.Commands.Reopen
{
    public class ReopenTicketCommandDto
    {
        public Guid TicketId { get; set; }
        public string Reason { get; set; } = "";
    }
}
