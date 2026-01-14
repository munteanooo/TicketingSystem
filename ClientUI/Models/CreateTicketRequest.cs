namespace ClientUI.Models
{
    public class CreateTicketRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = "Normal";
        public string Category { get; set; } = "General";
    }
}
