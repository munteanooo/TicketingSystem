using MediatR;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Interfaces;

namespace TicketingSystem.Application.Queries
{
    public class GetAllTicketsQuery : IRequest<List<TicketDto>>
    {
    }

    public class GetAllTicketsQueryHandler : IRequestHandler<GetAllTicketsQuery, List<TicketDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllTicketsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TicketDto>> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _unitOfWork.Tickets.GetAllAsync();
            return tickets.Select(MapToDto).ToList();
        }

        private TicketDto MapToDto(Ticket ticket)
        {
            return new TicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Priority = ticket.Priority,
                Status = ticket.Status,
                CreatedById = ticket.CreatedById,
                AssignedToId = ticket.AssignedToId,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                ResolvedAt = ticket.ResolvedAt
            };
        }
    }
}