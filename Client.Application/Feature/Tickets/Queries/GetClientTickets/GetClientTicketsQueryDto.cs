namespace Client.Application.Feature.Tickets.Queries.GetClientTickets
{
    public class GetClientTicketsQueryDto
    {
        public Guid ClientId { get; set; }

        // Opțional: filtre suplimentare, paginare etc.
        public string? Status { get; set; }        // "Open", "Resolved", etc.
        public string? Priority { get; set; }      // "Low", "Medium", "High"
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
