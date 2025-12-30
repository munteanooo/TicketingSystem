using Client.Application.Feature.Tickets.Commands.Create;
using Client.Application.Feature.Tickets.Commands.Delete;
using Client.Application.Feature.Tickets.Commands.Ticket;

public interface ITicketService
{
    Task<TicketCommandResponseDto> CreateAsync(CreateTicketCommand command);
    Task<bool> DeleteAsync(DeleteTicketCommand command);
}
