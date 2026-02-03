using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System.Reflection;

namespace TicketingSystem.Application.Contracts
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // MediatR pentru toate Handlers din Application
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Aici poți adăuga validatori, behaviors, policies, etc.

            return services;
        }
    }
}
