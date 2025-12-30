using MediatR;
using Client.Application.Feature.Tickets.Commands.Ticket;

namespace Client.Application.Feature.Tickets.Commands.Assign;
public record AssignTicketCommand(
    Guid TicketId,
    Guid AgentId
) : IRequest<TicketCommandResponseDto>;
