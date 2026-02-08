using MediatR;
using TicketingSystem.Application.Interfaces;

namespace TicketingSystem.Application.Tickets.Commands.CreateTicket
{
    public record CreateTicketCommand(CreateTicketCommandDto CommandDto)
        : IRequest<CreateTicketCommandResponseDto>, ICommand;
}
