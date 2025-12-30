namespace Client.Application.Feature.Tickets.Commands.UpdateStatus
{
    public class UpdateTicketStatusDto
    {
        public string NewStatus { get; set; }
        public string Message { get; set; }
    }
}