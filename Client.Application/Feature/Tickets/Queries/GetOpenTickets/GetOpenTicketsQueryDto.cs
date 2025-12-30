namespace Client.Application.Feature.Tickets.Queries.GetOpenTickets
{
    public class GetOpenTicketsQueryDto
    {
        public string? Priority { get; set; }
        public Guid? AgentId { get; set; }
    }
}
