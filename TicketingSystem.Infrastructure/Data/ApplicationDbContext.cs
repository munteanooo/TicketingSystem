namespace TicketingSystem.Infrastructure.Data
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using TicketingSystem.Domain.Entities;
    using TicketingSystem.Infrastructure.Identity;
    using TicketingSystem.Infrastructure.Configurations;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DomainUser> DomainUsers { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketMessage> TicketMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new DomainUserConfiguration());
            builder.ApplyConfiguration(new TicketConfiguration());
            builder.ApplyConfiguration(new TicketMessageConfiguration());
        }
    }
}