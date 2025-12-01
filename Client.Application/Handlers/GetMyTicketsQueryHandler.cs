using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using TicketingSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Client.Application.Queries;
using Client.Application.DTOs;
using TicketingSystem.Domain.Enums;

namespace Client.Application.Handlers;

public class GetMyTicketsQueryHandler : IRequestHandler<GetMyTicketsQuery, List<TicketDto>>
{
    private readonly ApplicationDbContext _context;

    public GetMyTicketsQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TicketDto>> Handle(GetMyTicketsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Tickets
            .Include(t => t.Client)
            .Where(t => t.ClientId == request.ClientId)
            .OrderByDescending(t => t.CreatedAt)
            .AsQueryable();

        if (request.Status.HasValue)
            query = query.Where(t => t.Status == request.Status.Value);

        var tickets = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TicketDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description.Length > 100
                    ? t.Description.Substring(0, 100) + "..."
                    : t.Description,
                Priority = t.Priority,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                ClosedAt = t.ClosedAt,
                ClosingNotes = t.ClosingNotes,
                ClientId = t.ClientId,
                ClientName = t.Client.FullName
            })
            .ToListAsync(cancellationToken);

        return tickets;
    }
}