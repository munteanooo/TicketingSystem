using MediatR;
using TicketingSystem.Application.Contracts.Exceptions;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Application.Tickets.Queries.GetTicketDetails
{
    public class GetTicketDetailsQueryHandler : IRequestHandler<GetTicketDetailsQuery, GetTicketDetailsQueryResponseDto>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ICurrentUser _currentUser;

        public GetTicketDetailsQueryHandler(
            ITicketRepository ticketRepository,
            ICurrentUser currentUser)
        {
            _ticketRepository = ticketRepository;
            _currentUser = currentUser;
        }

        public async Task<GetTicketDetailsQueryResponseDto> Handle(GetTicketDetailsQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketRepository.GetByIdAsync(request.TicketId, cancellationToken);

            if (ticket == null)
                throw NotFoundException.Create(nameof(Ticket), request.TicketId);

            // Verificare permisiuni: doar proprietarul, tehnicianul alocat sau adminul
            if (ticket.ClientId.ToString() != _currentUser.UserId
                && ticket.AssignedTechnicianId?.ToString() != _currentUser.UserId
                && !_currentUser.IsAdmin)
                throw ForbiddenException.InvalidOwner(nameof(Ticket));

            return MapToDto(ticket);
        }

        private GetTicketDetailsQueryResponseDto MapToDto(Ticket ticket)
        {
            return new GetTicketDetailsQueryResponseDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                Title = ticket.Title,
                Description = ticket.Description,
                Category = ticket.Category,
                Priority = ticket.Priority.ToString(),
                Status = ticket.Status.ToString(),
                ClientId = ticket.ClientId,
                ClientName = ticket.Client?.FullName,
                ClientEmail = ticket.Client?.Email,
                AssignedTechnicianId = ticket.AssignedTechnicianId,
                AssignedTechnicianName = ticket.AssignedTechnician?.FullName,
                ResolutionNote = ticket.ResolutionNote,
                ReopenReason = ticket.ReopenReason,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                AssignedAt = ticket.AssignedAt,
                ClosedAt = ticket.ClosedAt,
                ReopenedAt = ticket.ReopenedAt,
                MessageCount = ticket.Messages?.Count ?? 0,

                Messages = ticket.Messages?
                    .OrderBy(m => m.CreatedAt) 
                    .Select(m => new TicketMessageDto
                    {
                        Id = m.Id,
                        Content = m.Content,
                        AuthorId = m.AuthorId,
                        AuthorName = m.Author?.FullName ?? "Utilizator Sistem", 
                        CreatedAt = m.CreatedAt
                    }).ToList() ?? new List<TicketMessageDto>()
            };
        }
    }
}