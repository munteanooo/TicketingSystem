using MediatR;

namespace Client.Application.Feature.Tickets.Commands.Ticket;

public record TicketCommand(TicketCommandDto TicketDto)
    : IRequest<TicketCommandResponseDto>;
