using MediatR;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.Interfaces;

namespace TicketingSystem.Application.Commands
{
    public class CreateTicketCommand : IRequest<TicketDto>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketPriority Priority { get; set; }
        public int CreatedById { get; set; }
    }

    public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, TicketDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateTicketCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TicketDto> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = new Ticket(request.Title, request.Description, request.Priority, request.CreatedById);

            await _unitOfWork.Tickets.AddAsync(ticket);
            await _unitOfWork.SaveChangesAsync();

            return new TicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Priority = ticket.Priority,
                Status = ticket.Status,
                CreatedAt = ticket.CreatedAt
            };
        }
    }
}