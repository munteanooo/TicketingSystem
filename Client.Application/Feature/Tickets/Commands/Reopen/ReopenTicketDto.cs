namespace Client.Application.Feature.Tickets.Commands.Reopen
{
    public class ReopenTicketDto
    {
        public Guid TicketId { get; set; }
        public string Reason { get; set; } = "";
    }
}
