using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TicketingSystem.Infrastructure.Persistence
{
    /// <summary>
    /// Factory for creating DbContext at design time (for migrations)
    /// This is needed because EF Core tools don't have access to Program.cs
    /// during migration generation
    /// </summary>
    public class TicketingSystemDbContextFactory : IDesignTimeDbContextFactory<TicketingSystemDbContext>
    {
        public TicketingSystemDbContext CreateDbContext(string[] args)
        {
            // Hardcode connection string for development
            // This is only used by EF Core tools for migrations
            var connectionString = "Host=localhost;Port=5432;Database=TicketingSystem_Dev;Username=postgres;Password=1212";

            var optionsBuilder = new DbContextOptionsBuilder<TicketingSystemDbContext>();
            optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly("TicketingSystem.Infrastructure");
                npgsqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
                npgsqlOptions.CommandTimeout(30);
                npgsqlOptions.UseRelationalNulls();
            });

            return new TicketingSystemDbContext(optionsBuilder.Options);
        }
    }
}