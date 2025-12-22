namespace TicketingSystem.Infrastructure.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using TicketingSystem.Domain.Entities;

    public class DomainUserConfiguration : IEntityTypeConfiguration<DomainUser>
    {
        public void Configure(EntityTypeBuilder<DomainUser> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedNever();

            builder.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(x => x.Role)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.Property(x => x.CreatedAt).IsRequired();

            builder.HasMany(x => x.SubmittedTickets)
                .WithOne(t => t.Client)
                .HasForeignKey(t => t.ClientId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.AssignedTickets)
                .WithOne(t => t.AssignedToAgent)
                .HasForeignKey(t => t.AssignedToAgentId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.Messages)
                .WithOne(m => m.Author)
                .HasForeignKey(m => m.AuthorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.Role);
            builder.HasIndex(x => x.IsActive);
        }
    }
}