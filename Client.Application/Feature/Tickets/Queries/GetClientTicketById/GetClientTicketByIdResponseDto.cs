namespace Client.Application.Feature.Tickets.Queries.GetClientTicketById
{
    public class GetClientTicketByIdResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string Priority { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public List<TicketMessageDto>? Messages { get; set; }
    }

    public class TicketMessageDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = default!;
        public string Sender { get; set; } = default!; 
        public DateTime CreatedAt { get; set; }
    }
}
