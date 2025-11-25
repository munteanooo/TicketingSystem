using System;
using System.Collections.Generic;

namespace TicketingSystem.Domain.Entities
{
    public class TicketMessage
    {
        public int Id { get; set; }                    
        public string Content { get; set; }            
        public DateTime Created { get; set; } = DateTime.UtcNow;

        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }            

        public int UserId { get; set; }
        public User User { get; set; }                 

        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
        public TicketMessage() { }

        public TicketMessage(string content, int ticketId, int userId)
        {
            Content = content;
            TicketId = ticketId;
            UserId = userId;
            Created = DateTime.UtcNow;
        }
    }
}
