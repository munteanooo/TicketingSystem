namespace Client.Application.Feature.Tickets.Queries.GetAllTickets
{
    public class GetAllTicketsQueryDto
    {
        // Filtre opționale
        public string? Status { get; set; }       // ex: "Open", "Resolved", "Reopened"
        public string? Priority { get; set; }     // ex: "Low", "Medium", "High", "Critical"
        public Guid? AgentId { get; set; }        // filtrează tichetele atribuite unui agent
        public Guid? ClientId { get; set; }       // filtrează tichetele unui client

        // Sortare
        public string? OrderBy { get; set; }      // ex: "Priority", "CreatedAt", "UpdatedAt"
        public bool OrderDescending { get; set; } // true = desc, false = asc

        // Paginare (optional)
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
