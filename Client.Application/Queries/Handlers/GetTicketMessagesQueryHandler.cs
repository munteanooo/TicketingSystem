using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Infrastructure.Data;

namespace Client.Application.Queries.Handlers
{
    public class GetTicketMessagesQueryHandler : IRequestHandler<GetTicketMessagesQuery, List<TicketMessageDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetTicketMessagesQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TicketMessageDto>> Handle(GetTicketMessagesQuery request, CancellationToken cancellationToken)
        {
            var messages = await _context.TicketMessages
                .Where(m => m.TicketId == request.TicketId)
                .Include(m => m.Author)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync(cancellationToken);

            return messages.Select(MapToDto).ToList();
        }

        private TicketMessageDto MapToDto(TicketingSystem.Domain.Entities.TicketMessage message)
        {
            return new TicketMessageDto
            {
                Id = message.Id,
                TicketId = message.TicketId,
                AuthorId = message.AuthorId,
                AuthorName = message.Author?.FullName,
                Content = message.Content,
                CreatedAt = message.CreatedAt
            };
        }
    }
}
