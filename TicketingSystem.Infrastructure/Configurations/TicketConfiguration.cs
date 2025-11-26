using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Infrastructure.Data.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(2000);

            builder.Property(t => t.Priority)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.IsResolved)
                .IsRequired();

            builder.HasOne(t => t.User)
                .WithMany()  
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.TechSupport)
                .WithMany()  
                .HasForeignKey(t => t.TechSupportId)
                .IsRequired(false)  
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}