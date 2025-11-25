using Microsoft.AspNetCore.Identity;

namespace TicketingSystem.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
