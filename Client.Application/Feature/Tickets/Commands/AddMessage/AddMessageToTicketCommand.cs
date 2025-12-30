using MediatR;

namespace Client.Application.Feature.Tickets.Commands.AddMessage
{
    public record AddMessageToTicketCommand(
        Guid TicketId,
        Guid AuthorId,
        string AuthorName,
        string Content
    ) : IRequest<AddMessageToTicketCommandResponseDto>;
}
