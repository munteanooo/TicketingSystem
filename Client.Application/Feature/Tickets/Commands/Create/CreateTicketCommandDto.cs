namespace Client.Application.Feature.Tickets.Commands.Create
{
    public class CreateTicketCommandDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Priority { get; set; }
    }
}