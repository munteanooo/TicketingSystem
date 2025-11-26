using Client.Application.Commands;
using MediatR;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Infrastructure.Data;

namespace Client.Application.Handlers
{
    public class AddMessageToTicketCommandHandler : IRequestHandler<AddMessageToTicketCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public AddMessageToTicketCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AddMessageToTicketCommand request, CancellationToken ct)
        {
            var message = new TicketMessage
            {
                Content = request.Content,
                TicketId = request.TicketId,
                UserId = request.ClientId,
                Created = DateTime.UtcNow
            };

            _context.TicketMessages.Add(message);
            await _context.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}