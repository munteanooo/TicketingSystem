using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketingSystem.Domain.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Enums.TicketPriority Priority { get; set; }
        public bool IsResolved { get; set; } = false;
        public DateTime Created { get; set; } = DateTime.Now;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int? TechSupportId { get; set; }
        public User? TechSupport { get; set; }
        public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
    }
}
