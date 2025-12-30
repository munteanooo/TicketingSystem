namespace Client.Application.Feature.Tickets.Commands.Create
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Client.Application.Contracts.Persistence;
    using MediatR;
    using Client.Application.Feature.Tickets.Commands.Ticket;
    using TicketingSystem.Domain.Entities;
    using TicketingSystem.Domain.Enums;

    public class CreateTicketCommandHandler(
    ITicketRepository _ticketRepository,
    IUserRepository _userRepository)
    : IRequestHandler<CreateTicketCommand, TicketCommandResponseDto>
    {
        public async Task<TicketCommandResponseDto> Handle(
            CreateTicketCommand request,
            CancellationToken cancellationToken)
        {
            var client = await _userRepository.GetByIdAsync(request.ClientId);
            if (client == null)
                throw new Exception("Client not found");

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                TicketNumber = GenerateTicketNumber(),
                Title = request.Title,
                Description = request.Description,
                Category = request.Category,
                Priority = Enum.Parse<TicketPriority>(request.Priority, true),
                Status = TicketStatus.Open,
                ClientId = client.Id,
                Client = client,
                CreatedAt = DateTime.UtcNow
            };

            await _ticketRepository.AddAsync(ticket);

            return MapToDto(ticket);
        }

        private string GenerateTicketNumber()
        {
            var year = DateTime.UtcNow.Year;
            var random = new Random();
            return $"TK-{year}-{random.Next(10000, 99999)}";
        }
        private TicketCommandResponseDto MapToDto(Ticket ticket)
        {
            return new TicketCommandResponseDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                Title = ticket.Title,
                Description = ticket.Description,
                Priority = ticket.Priority.ToString(),
                Status = ticket.Status.ToString(),
                Category = ticket.Category,
                ClientId = ticket.ClientId,
                CreatedAt = ticket.CreatedAt
            };
        }
    }
}