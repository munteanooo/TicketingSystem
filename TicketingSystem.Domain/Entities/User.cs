using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketingSystem.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public Enums.UserRole Role { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<TicketMessage> Messages { get; set; }
    }
}
