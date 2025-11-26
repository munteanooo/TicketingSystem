using TicketingSystem.Domain.Entities;

namespace Client.Application.DTOs
{
    public class TicketDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public bool IsResolved { get; set; }

        public List<TicketMessageDto> Messages { get; set; } = new();
    }

    public class TicketMessageDto
    {
        public string Content { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public bool IsFromClient { get; set; }
    }
}