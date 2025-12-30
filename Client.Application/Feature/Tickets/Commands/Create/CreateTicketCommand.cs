namespace Client.Application.Feature.Tickets.Commands.Create;

using MediatR;
using Client.Application.Feature.Tickets.Commands.Ticket;

public record CreateTicketCommand(
    Guid ClientId,
    string Title,
    string Description,
    string Category,
    string Priority
) : IRequest<TicketCommandResponseDto>;
