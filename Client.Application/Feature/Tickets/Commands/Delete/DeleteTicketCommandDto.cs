namespace Client.Application.Feature.Tickets.Commands.Delete
{
    public class DeleteTicketCommandDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
