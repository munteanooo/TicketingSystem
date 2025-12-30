using MediatR;

namespace Client.Application.Feature.Tickets.Commands.Reopen
{
    public record ReopenTicketCommand(ReopenTicketCommandDto ReopenDto)
        : IRequest<ReopenTicketCommandResponseDto>;
}
