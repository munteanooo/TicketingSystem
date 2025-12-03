using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Infrastructure.Data;
using Client.Application.Queries;
using Client.Application.DTOs;

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
        Console.WriteLine($"GetMyTicketsQuery pentru ClientId: {request.ClientId}");

        var query = _context.Tickets
            .Include(t => t.Client)
            .Where(t => t.ClientId == request.ClientId)
            .AsQueryable();

        var count = await query.CountAsync(cancellationToken);
        Console.WriteLine($"Tichete găsite pentru client {request.ClientId}: {count}");

        if (request.Status.HasValue)
            query = query.Where(t => t.Status == request.Status.Value);

        var tickets = await query
            .OrderByDescending(t => t.CreatedAt)
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

        Console.WriteLine($"Tichete returnate: {tickets.Count}");

        return tickets;
    }
}