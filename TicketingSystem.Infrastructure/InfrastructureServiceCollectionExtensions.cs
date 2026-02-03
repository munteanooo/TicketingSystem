using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Infrastructure.Persistence;
using TicketingSystem.Infrastructure.Persistence.Repositories;
using TicketingSystem.Infrastructure.Services; 

namespace TicketingSystem.Infrastructure
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found in configuration.");

            // 1. Configurare DbContext
            RegisterDbContext(services, connectionString);

            // 2. Configurare Infrastructură Web (Esențial pentru ICurrentUser)
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUser, CurrentUserService>();

            // 3. Înregistrare Repositories
            RegisterRepositories(services);

            return services;
        }

        private static void RegisterDbContext(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<TicketingSystemDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly("TicketingSystem.Infrastructure");
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);

                    npgsqlOptions.CommandTimeout(30);
                });

#if DEBUG
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
#endif
            });

            // Permite injectarea bazei de date ca DbContext generic dacă este nevoie
            services.AddScoped<DbContext>(provider => provider.GetRequiredService<TicketingSystemDbContext>());
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            // Înregistrare simplificată (nu este nevoie de factory function dacă constructorul este simplu)
            // .NET DI va rezolva automat TicketingSystemDbContext în constructorul repository-ului
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}