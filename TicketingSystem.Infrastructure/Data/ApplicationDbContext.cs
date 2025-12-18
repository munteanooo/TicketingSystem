using Microsoft.EntityFrameworkCore;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketMessage> TicketMessages { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ApplicationUser Configuration
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Role).HasConversion<int>();
                entity.Property(u => u.IsActive).HasDefaultValue(true);

                entity.HasIndex(u => u.Email).IsUnique();

                entity.HasMany(u => u.CreatedTickets)
                    .WithOne(t => t.CreatedBy)
                    .HasForeignKey(t => t.CreatedById)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(u => u.AssignedTickets)
                    .WithOne(t => t.AssignedTo)
                    .HasForeignKey(t => t.AssignedToId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(u => u.Messages)
                    .WithOne(m => m.Author)
                    .HasForeignKey(m => m.AuthorId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Ticket Configuration
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title).IsRequired().HasMaxLength(200);
                entity.Property(t => t.Description).IsRequired().HasMaxLength(5000);
                entity.Property(t => t.Priority).HasConversion<int>();
                entity.Property(t => t.Status).HasConversion<int>();

                // PostgreSQL - NOW() cu timezone UTC
                entity.Property(t => t.CreatedAt)
                    .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

                entity.HasIndex(t => t.Status);
                entity.HasIndex(t => t.Priority);
                entity.HasIndex(t => t.CreatedById);
                entity.HasIndex(t => t.AssignedToId);

                entity.HasMany(t => t.Messages)
                    .WithOne(m => m.Ticket)
                    .HasForeignKey(m => m.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // TicketMessage Configuration
            modelBuilder.Entity<TicketMessage>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Content).IsRequired().HasMaxLength(3000);

                // PostgreSQL - NOW() cu timezone UTC
                entity.Property(m => m.CreatedAt)
                    .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

                entity.HasIndex(m => m.TicketId);
                entity.HasIndex(m => m.AuthorId);
            });
        }
    }
}