using MediatR;
using Microsoft.EntityFrameworkCore;
using Client.Application.Queries;
using Client.Application.DTOs;
using TicketingSystem.Infrastructure.Data;

namespace Client.Application.Handlers;

public class GetTicketDetailsQueryHandler : IRequestHandler<GetTicketDetailsQuery, TicketDetailsDto?>
{
    private readonly ApplicationDbContext _context;

    public GetTicketDetailsQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TicketDetailsDto?> Handle(GetTicketDetailsQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _context.Tickets
            .Where(t => t.Id == request.TicketId && t.ClientId == request.ClientId)
            .Include(t => t.Client)
            .Include(t => t.AssignedTo)
            .Include(t => t.Messages)
                .ThenInclude(m => m.User)
            .Include(t => t.Messages)
                .ThenInclude(m => m.Attachments)
            .FirstOrDefaultAsync(cancellationToken);

        return ticket == null ? null : new TicketDetailsDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Priority = ticket.Priority,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            ClosedAt = ticket.ClosedAt,
            ClosingNotes = ticket.ClosingNotes,
            ClientId = ticket.ClientId,
            ClientName = ticket.Client.FullName,
            AssignedToId = ticket.AssignedToId,
            AssignedToName = ticket.AssignedTo?.FullName,
            Messages = ticket.Messages.Select(m => new TicketMessageDto
            {
                Id = m.Id,
                Content = m.Content,
                CreatedAt = m.CreatedAt,
                IsInternal = m.IsInternal,
                UserId = m.UserId,
                UserName = m.User.FullName,
                Attachments = m.Attachments.Select(a => new AttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    ContentType = a.ContentType,
                    FileSize = a.FileSize,
                    UploadedAt = a.UploadedAt
                }).ToList()
            }).OrderBy(m => m.CreatedAt).ToList()
        };
    }
}