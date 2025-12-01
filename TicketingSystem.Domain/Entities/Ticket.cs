using System;
using System.Collections.Generic;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Domain.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string ClosingNotes { get; set; } = string.Empty;

        public int ClientId { get; set; }
        public User Client { get; set; } = null!;

        public int? AssignedToId { get; set; }
        public User? AssignedTo { get; set; }

        public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

        public bool IsClosed => Status == TicketStatus.Closed || Status == TicketStatus.Resolved;
    }
}