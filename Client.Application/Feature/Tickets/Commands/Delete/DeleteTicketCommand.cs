using MediatR;

namespace Client.Application.Feature.Tickets.Commands.Delete;

public record DeleteTicketCommand(Guid TicketId) : IRequest<bool>;
