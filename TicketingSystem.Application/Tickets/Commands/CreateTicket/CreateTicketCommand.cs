using MediatR;

namespace TicketingSystem.Application.Tickets.Commands.CreateTicket
{
    public record CreateTicketCommand (CreateTicketCommandDto CommandDto) 
        : IRequest<CreateTicketCommandResponseDto>;
}
