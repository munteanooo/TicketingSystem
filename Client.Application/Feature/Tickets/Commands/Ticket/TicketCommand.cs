using MediatR;

namespace Client.Application.Feature.Tickets.Commands.Ticket;

public record UpdateTicketCommand(TicketCommandDto TicketDto)
    : IRequest<TicketCommandResponseDto>;
