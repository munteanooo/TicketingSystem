using Client.Application.Feature.Tickets.Commands.Ticket;
using MediatR;

namespace Client.Application.Feature.Tickets.Commands.Close;

public record CloseTicketCommand (Guid TicketId) : IRequest<TicketCommandResponseDto>;
