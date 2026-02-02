using MediatR;
using TicketingSystem.Application.Contracts.Exceptions;
using TicketingSystem.Application.Contracts.Interfaces;
using TicketingSystem.Domain.Entities;

namespace TicketingSystem.Application.Tickets.Commands.CreateTicket
{
    public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, CreateTicketCommandResponseDto>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;

        public CreateTicketCommandHandler(
            ITicketRepository ticketRepository,
            IUserRepository userRepository)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
        }

        public async Task<CreateTicketCommandResponseDto> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            var client = await _userRepository.GetByIdAsync(request.CommandDto.ClientId, cancellationToken);
            if (client == null)
                throw NotFoundException.Create(nameof(User), request.CommandDto.ClientId);

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                TicketNumber = GenerateTicketNumber(),
                Title = request.CommandDto.Title,
                Description = request.CommandDto.Description,
                Category = request.CommandDto.Category,
                Priority = Enum.Parse<TicketPriority>(request.CommandDto.Priority, true),
                Status = TicketStatus.Open,
                ClientId = client.Id,
                Client = client,
                CreatedAt = DateTime.UtcNow
            };
            await _ticketRepository.AddAsync(ticket, cancellationToken);
            return MapToDto(ticket);
        }

        private string GenerateTicketNumber()
        {
            var year = DateTime.UtcNow.Year;
            var random = new Random();
            return $"TK-{year}-{random.Next(10000, 99999)}";
        }

        private CreateTicketCommandResponseDto MapToDto(Ticket ticket)
        {
            return new CreateTicketCommandResponseDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                Title = ticket.Title,
                Description = ticket.Description,
                Category = ticket.Category,
                Priority = ticket.Priority.ToString(),
                Status = ticket.Status.ToString(),
                ClientId = ticket.ClientId,
                CreatedAt = ticket.CreatedAt
            };
        }
    }
}