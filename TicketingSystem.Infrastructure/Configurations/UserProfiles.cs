using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketingSystem.Domain.Entities;

public class UserProfiles : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(u => u.PhoneNumber)
               .HasMaxLength(20);

        builder.HasIndex(u => u.Email).IsUnique();

        // Relațiile corecte pentru Ticket
        builder.HasMany(u => u.SubmittedTickets)
               .WithOne(t => t.Client)
               .HasForeignKey(t => t.ClientId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.AssignedTickets)
               .WithOne(t => t.AssignedToAgent)
               .HasForeignKey(t => t.AssignedToAgentId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Messages)
               .WithOne(m => m.Author)
               .HasForeignKey(m => m.AuthorId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
