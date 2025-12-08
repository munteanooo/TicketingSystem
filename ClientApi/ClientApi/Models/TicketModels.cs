namespace ClientApi.Models
{
    // Pentru creare ticket
    public class CreateTicketRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    // Pentru afișare ticket
    public class TicketResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
    }
}