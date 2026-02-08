using MediatR;
using TicketingSystem.Application.Contracts.Exceptions;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Application.Tickets.Queries.GetTicketDetails;

public class GetTicketDetailsQueryHandler : IRequestHandler<GetTicketDetailsQuery, GetTicketDetailsQueryResponseDto>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ICurrentUser _currentUser;

    public GetTicketDetailsQueryHandler(ITicketRepository ticketRepository, ICurrentUser currentUser)
    {
        _ticketRepository = ticketRepository;
        _currentUser = currentUser;
    }

    public async Task<GetTicketDetailsQueryResponseDto> Handle(GetTicketDetailsQuery request, CancellationToken cancellationToken)
    {
        bool isPowerUser = _currentUser.IsAdmin || _currentUser.IsTechnician;

        var ticket = isPowerUser
            ? await _ticketRepository.GetByIdForAdminAsync(request.TicketId, cancellationToken)
            : await _ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);

        if (ticket == null) throw new NotFoundException("Ticket negăsit");

        return new GetTicketDetailsQueryResponseDto
        {
            Id = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            Title = ticket.Title,
            Description = ticket.Description, // Acesta este primul mesaj/descrierea
            Category = ticket.Category ?? "General",
            Priority = ticket.Priority.ToString(),
            ClientId = ticket.ClientId,
            CreatedAt = ticket.CreatedAt,
            Status = ticket.Status.ToString(),
            Messages = ticket.Messages.Select(m => new TicketMessageDto
            {
                Content = m.Content,
                AuthorName = m.Author?.FullName ?? "Sistem",
                CreatedAt = m.CreatedAt,
                IsFromSupport = m.Author != null && (m.Author.Role == "Admin" || m.Author.Role == "TechSupport")
            }).ToList()
        };
    }
}