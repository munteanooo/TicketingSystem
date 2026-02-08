using MediatR;
using TicketingSystem.Application.Interfaces;
using TicketingSystem.Infrastructure.Persistence;

namespace TicketingSystem.Infrastructure.MediatR
{
    public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly TicketingSystemDbContext _dbContext;

        public UnitOfWorkBehavior(TicketingSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Execute handler
            var response = await next();

            // Persist only for requests that are ICommand (explicit marker)
            if (request is ICommand)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return response;
        }
    }
}
