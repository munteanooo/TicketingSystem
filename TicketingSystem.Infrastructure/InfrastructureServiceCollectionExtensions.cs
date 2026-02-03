using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Infrastructure.Persistence;
using TicketingSystem.Infrastructure.Persistence.Repositories;

namespace TicketingSystem.Infrastructure
{
    /// <summary>
    /// Extension methods for registering Infrastructure services in the DI container
    /// Called from Program.cs to register all infrastructure dependencies
    /// </summary>
    public static class InfrastructureServiceCollectionExtensions
    {
        /// <summary>
        /// Add Infrastructure services to the DI container
        /// This includes DbContext, Repositories, and PostgreSQL configuration
        /// 
        /// Usage in Program.cs:
        /// builder.Services.AddInfrastructure(builder.Configuration);
        /// </summary>
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // Get connection string from configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found in configuration. " +
                    "Ensure appsettings.json contains the connection string.");

            // Register DbContext with PostgreSQL provider
            RegisterDbContext(services, connectionString);

            // Register Repositories
            RegisterRepositories(services);

            return services;
        }

        /// <summary>
        /// Register the DbContext with PostgreSQL provider
        /// Configures connection pooling and retry policy for resilience
        /// </summary>
        private static void RegisterDbContext(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<TicketingSystemDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    // Specify the assembly containing migrations
                    npgsqlOptions.MigrationsAssembly("TicketingSystem.Infrastructure");

                    // Enable retry on failure for transient database errors
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,                          // Retry up to 3 times
                        maxRetryDelay: TimeSpan.FromSeconds(5),                   // Wait up to 5 seconds between retries
                        errorCodesToAdd: null);                    // Add custom error codes if needed

                    // Configure command timeout
                    npgsqlOptions.CommandTimeout(30);             // 30 second timeout per command

                    // Enable connection resilience
                    npgsqlOptions.UseRelationalNulls();           // Use SQL NULL semantics
                });

                // Enable lazy loading of navigation properties (optional)
                // options.UseLazyLoadingProxies();

                // Configure query behavior in development
#if DEBUG
                options.EnableDetailedErrors();                   // More detailed error messages
                options.EnableSensitiveDataLogging();             // Log sensitive data (passwords, etc.) - DEVELOPMENT ONLY!
#endif
            });

            // Register the DbContext for explicit dependency injection
            services.AddScoped<DbContext>(provider => provider.GetRequiredService<TicketingSystemDbContext>());
        }

        /// <summary>
        /// Register all repository implementations in the DI container
        /// Each repository interface is mapped to its implementation
        /// Scoped lifetime ensures a new instance per request/scope
        /// </summary>
        private static void RegisterRepositories(IServiceCollection services)
        {
            // Register Ticket Repository
            services.AddScoped<ITicketRepository, TicketRepository>(provider =>
            {
                var context = provider.GetRequiredService<TicketingSystemDbContext>();
                return new TicketRepository(context);
            });

            // Register User Repository
            services.AddScoped<IUserRepository, UserRepository>(provider =>
            {
                var context = provider.GetRequiredService<TicketingSystemDbContext>();
                return new UserRepository(context);
            });
        }
    }
}