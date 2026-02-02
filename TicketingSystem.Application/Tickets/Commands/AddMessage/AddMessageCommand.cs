using MediatR;

namespace TicketingSystem.Application.Tickets.Commands.AddMessage
{
    public record AddMessageCommand(AddMessageCommandDto CommandDto)
        : IRequest<AddMessageCommandResponseDto>;
}