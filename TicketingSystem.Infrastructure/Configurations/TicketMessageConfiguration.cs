using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Infrastructure.Data.Configurations
{
    public class TicketMessageConfiguration : IEntityTypeConfiguration<TicketMessage>
    {
        public void Configure(EntityTypeBuilder<TicketMessage> builder)
        {
            builder.HasKey(tm => tm.Id);

            builder.Property(tm => tm.Content)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(tm => tm.Created)
                .IsRequired();

            builder.Property(tm => tm.TicketId)
                .IsRequired();

            builder.Property(tm => tm.UserId)
                .IsRequired();

            builder.HasOne(tm => tm.Ticket)
                .WithMany(t => t.Messages)
                .HasForeignKey(tm => tm.TicketId)
                .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(tm => tm.User)
                .WithMany()
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasIndex(tm => tm.TicketId);
            builder.HasIndex(tm => tm.Created);
            builder.HasIndex(tm => tm.UserId);
        }
    }
}