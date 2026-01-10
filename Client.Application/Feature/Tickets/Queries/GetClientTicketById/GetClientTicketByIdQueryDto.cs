namespace Client.Application.Feature.Tickets.Queries.GetClientTicketById
{
    public class GetClientTicketByIdQueryDto
    {
        public Guid TicketId { get; set; }
        public Guid ClientId { get; set; } 
    }
}
