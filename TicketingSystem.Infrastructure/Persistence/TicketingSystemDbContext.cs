using Microsoft.EntityFrameworkCore;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Infrastructure.Persistence
{
    /// <summary>
    /// Entity Framework Core DbContext for TicketingSystem
    /// Manages database connections and entity mappings
    /// </summary>
    public class TicketingSystemDbContext : DbContext
    {
        public TicketingSystemDbContext(DbContextOptions<TicketingSystemDbContext> options)
            : base(options)
        {
        }

        #region DbSets
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Ticket> Tickets { get; set; } = null!;
        public DbSet<TicketMessage> TicketMessages { get; set; } = null!;
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            ConfigureUserEntity(modelBuilder);

            // Configure Ticket entity
            ConfigureTicketEntity(modelBuilder);

            // Configure TicketMessage entity
            ConfigureTicketMessageEntity(modelBuilder);

            // Convert enums to strings
            ConfigureEnumConversions(modelBuilder);
        }

        /// <summary>
        /// Configure User entity mappings and relationships
        /// </summary>
        private static void ConfigureUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                // Primary key
                entity.HasKey(e => e.Id);

                // Column configurations
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.PasswordHash)
                    .IsRequired();

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Indexes
                entity.HasIndex(e => e.Email)
                    .IsUnique();

                // Relationships
                entity.HasMany(e => e.CreatedTickets)
                    .WithOne(t => t.Client)
                    .HasForeignKey(t => t.ClientId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Tickets_Users_ClientId");

                entity.HasMany(e => e.AssignedTickets)
                    .WithOne(t => t.AssignedTechnician)
                    .HasForeignKey(t => t.AssignedTechnicianId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Tickets_Users_TechnicianId");

                entity.HasMany(e => e.Messages)
                    .WithOne(m => m.Author)
                    .HasForeignKey(m => m.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Messages_Users_AuthorId");
            });
        }

        /// <summary>
        /// Configure Ticket entity mappings and relationships
        /// </summary>
        private static void ConfigureTicketEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>(entity =>
            {
                // Primary key
                entity.HasKey(e => e.Id);

                // Column configurations
                entity.Property(e => e.TicketNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Description)
                    .IsRequired();

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Priority)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ResolutionNote)
                    .HasMaxLength(1000);

                entity.Property(e => e.ReopenReason)
                    .HasMaxLength(1000);

                // Indexes for query performance
                entity.HasIndex(e => e.TicketNumber)
                    .IsUnique();

                entity.HasIndex(e => e.ClientId)
                    .HasDatabaseName("IX_Tickets_ClientId");

                entity.HasIndex(e => e.AssignedTechnicianId)
                    .HasDatabaseName("IX_Tickets_AssignedTechnicianId");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("IX_Tickets_Status");

                entity.HasIndex(e => e.Priority)
                    .HasDatabaseName("IX_Tickets_Priority");

                entity.HasIndex(e => e.CreatedAt)
                    .HasDatabaseName("IX_Tickets_CreatedAt");

                // Relationships
                entity.HasOne(e => e.Client)
                    .WithMany(u => u.CreatedTickets)
                    .HasForeignKey(e => e.ClientId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Tickets_Users_ClientId");

                entity.HasOne(e => e.AssignedTechnician)
                    .WithMany(u => u.AssignedTickets)
                    .HasForeignKey(e => e.AssignedTechnicianId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_Tickets_Users_TechnicianId");

                entity.HasMany(e => e.Messages)
                    .WithOne(m => m.Ticket)
                    .HasForeignKey(m => m.TicketId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Messages_Tickets_TicketId");
            });
        }

        /// <summary>
        /// Configure TicketMessage entity mappings and relationships
        /// </summary>
        private static void ConfigureTicketMessageEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketMessage>(entity =>
            {
                // Primary key
                entity.HasKey(e => e.Id);

                // Column configurations
                entity.Property(e => e.Content)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Indexes for query performance
                entity.HasIndex(e => e.TicketId)
                    .HasDatabaseName("IX_Messages_TicketId");

                entity.HasIndex(e => e.AuthorId)
                    .HasDatabaseName("IX_Messages_AuthorId");

                entity.HasIndex(e => e.CreatedAt)
                    .HasDatabaseName("IX_Messages_CreatedAt");

                // Relationships
                entity.HasOne(e => e.Ticket)
                    .WithMany(t => t.Messages)
                    .HasForeignKey(e => e.TicketId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Messages_Tickets_TicketId");

                entity.HasOne(e => e.Author)
                    .WithMany(u => u.Messages)
                    .HasForeignKey(e => e.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Messages_Users_AuthorId");
            });
        }

        /// <summary>
        /// Configure enum conversions to strings in database
        /// </summary>
        private static void ConfigureEnumConversions(ModelBuilder modelBuilder)
        {
            // Store enums as strings instead of integers
            modelBuilder.Entity<Ticket>()
                .Property(e => e.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Ticket>()
                .Property(e => e.Priority)
                .HasConversion<string>();
        }

        /// <summary>
        /// Override SaveChangesAsync to add audit fields
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<User>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            var ticketEntries = ChangeTracker.Entries<Ticket>();
            foreach (var entry in ticketEntries)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}