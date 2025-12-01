using System;
using System.Collections.Generic;

namespace TicketingSystem.Domain.Entities
{
    public class TicketMessage
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsInternal { get; set; } = false;

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

        public TicketMessage() { }

        public TicketMessage(string content, int ticketId, int userId, bool isInternal = false)
        {
            Content = content;
            TicketId = ticketId;
            UserId = userId;
            IsInternal = isInternal;
        }
    }
}