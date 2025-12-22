namespace ClientApi.Models
{
    public class CreateTicketRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Priority { get; set; }
    }

    public class TicketResponse
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public Guid? AssignedToAgentId { get; set; }
        public string AssignedToAgentName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }

    public class TicketDetailsResponse
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public Guid? AssignedToAgentId { get; set; }
        public string AssignedToAgentName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public List<TicketMessageResponse> Messages { get; set; } = new();
    }

    public class TicketMessageResponse
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AddMessageRequest
    {
        public string Content { get; set; }
    }

    public class UpdateTicketStatusRequest
    {
        public string NewStatus { get; set; }
        public string Message { get; set; }
    }
}