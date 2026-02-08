using MediatR;
using TicketingSystem.Application.Interfaces;

namespace TicketingSystem.Application.Tickets.Commands.AddMessage
{
    public record AddMessageCommand(AddMessageCommandDto CommandDto)
        : IRequest<AddMessageCommandResponseDto>, ICommand;
}