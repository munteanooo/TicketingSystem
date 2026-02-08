using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Application.Interfaces;

namespace TicketingSystem.Infrastructure.Persistence
{
    public class TicketingSystemDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IApplicationDbContext
    {
        public TicketingSystemDbContext(DbContextOptions<TicketingSystemDbContext> options)
            : base(options) { }

        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<TicketMessage> TicketMessages => Set<TicketMessage>();

        DbSet<User> IApplicationDbContext.Users => base.Users;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("AspNetUsers");
            modelBuilder.Entity<IdentityRole<Guid>>().ToTable("AspNetRoles");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("AspNetUserRoles");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("AspNetUserClaims");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("AspNetUserLogins");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("AspNetUserTokens");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("AspNetRoleClaims");
            modelBuilder.Entity<Ticket>().ToTable("Tickets");
            modelBuilder.Entity<TicketMessage>().ToTable("TicketMessages");

            ConfigureUserEntity(modelBuilder);
            ConfigureTicketEntity(modelBuilder);
            ConfigureTicketMessageEntity(modelBuilder);
            ConfigureEnumConversions(modelBuilder);
        }

        private static void ConfigureUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever();
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);

                entity.HasMany(e => e.CreatedTickets)
                    .WithOne(t => t.Client)
                    .HasForeignKey(t => t.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.AssignedTickets)
                    .WithOne(t => t.AssignedTechnician)
                    .HasForeignKey(t => t.AssignedTechnicianId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }

        private static void ConfigureTicketEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TicketNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.TicketNumber).IsUnique();

                entity.HasOne(e => e.Client)
                    .WithMany(u => u.CreatedTickets)
                    .HasForeignKey(e => e.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.AssignedTechnician)
                    .WithMany(u => u.AssignedTickets)
                    .HasForeignKey(e => e.AssignedTechnicianId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }

        private static void ConfigureTicketMessageEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Ticket)
                    .WithMany(t => t.Messages)
                    .HasForeignKey(e => e.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureEnumConversions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>().Property(e => e.Status).HasConversion<string>();
            modelBuilder.Entity<Ticket>().Property(e => e.Priority).HasConversion<string>();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            foreach (var entry in ChangeTracker.Entries<User>().Where(e => e.State == EntityState.Modified))
                entry.Entity.UpdatedAt = now;

            foreach (var entry in ChangeTracker.Entries<Ticket>().Where(e => e.State == EntityState.Modified))
                entry.Entity.UpdatedAt = now;

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}