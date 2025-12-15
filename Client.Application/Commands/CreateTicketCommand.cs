namespace TicketingSystem.Application.Commands
{
    using System;
    using MediatR;
    using TicketingSystem.Application.DTOs;

    public class CreateTicketCommand : IRequest<TicketDto>
    {
        public Guid ClientId { get; set; }
        public CreateTicketDto Ticket { get; set; }
    }
}