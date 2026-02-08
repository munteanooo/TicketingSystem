using TicketingSystem.Application.Tickets.Commands.CreateTicket;
using TicketingSystem.Application.Tickets.Commands.AddMessage;
using TicketingSystem.Application.Tickets.Queries.GetClientTickets;
using TicketingSystem.Application.Tickets.Queries.GetTechTickets;
using TicketingSystem.Blazor.Models.DTOs;

namespace TicketingSystem.Blazor.Services.Interfaces;

public interface ITicketService
{
    // Liste tichete
    Task<List<GetClientTicketsQueryResponseDto>> GetTickets(); // Toate tichetele (Istoric)
    Task<List<GetTechTicketsQueryResponseDto>> GetUnassignedTickets(); // Noi/Nepreluate
    Task<List<GetTechTicketsQueryResponseDto>> GetMyAssignedTickets(); // În lucru de către mine

    // Detalii și Acțiuni
    Task<TicketDto?> GetTicketById(Guid id);
    Task<CreateTicketCommandResponseDto?> CreateTicket(CreateTicketCommandDto ticketDto);
    Task<bool> AssignTicket(Guid ticketId);
    Task<bool> CloseTicket(Guid ticketId);
    Task<bool> UpdateTicketStatus(Guid ticketId, string status);

    // Mesaje
    Task<AddMessageCommandResponseDto?> AddMessage(Guid ticketId, string content);
}