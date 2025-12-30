using Client.Application.Feature.Tickets.Commands.Ticket;
using MediatR;
namespace Client.Application.Feature.Tickets.Commands.UpdateStatus;

public record UpdateTicketStatusCommand(UpdateTicketStatusCommandDto TicketStatusDto, Guid TicketId)
    : IRequest<TicketCommandResponseDto>;