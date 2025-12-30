using Client.Application.Feature.Tickets.Commands.AddMessage;
using Client.Application.Feature.Tickets.Commands.Create;
using Client.Application.Feature.Tickets.Commands.UpdateStatus;
using Client.Application.Feature.Tickets.Queries;
using Client.Application.Feature.Tickets.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Client.Application.Feature.Tickets.Commands.Delete;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/tickets
    [HttpGet]
    public async Task<IActionResult> GetAllTickets()
    {
        var tickets = await _mediator.Send(new GetAllTicketsQuery());
        return Ok(tickets);
    }

    // GET: api/tickets/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTicketById(Guid id)
    {
        var ticket = await _mediator.Send(new GetTicketByIdQuery { TicketId = id });
        if (ticket == null) return NotFound();
        return Ok(ticket);
    }

    // POST: api/tickets
    [HttpPost]
    public async Task<IActionResult> CreateTicket(CreateTicketCommand command)
    {
        var ticket = await _mediator.Send(command);
        return Ok(ticket);
    }

    // PUT: api/tickets/{id}/status
    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateTicketStatus(Guid id, UpdateTicketStatusCommand command)
    {
        command.TicketId = id;
        var updatedTicket = await _mediator.Send(command);
        if (updatedTicket == null) return NotFound();
        return Ok(updatedTicket);
    }

    // DELETE: api/tickets/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTicket(Guid id)
    {
        var result = await _mediator.Send(new DeleteTicketCommand(id));
        if (!result) return NotFound();
        return NoContent();
    }

    // POST: api/tickets/{id}/messages
    [HttpPost("{id:guid}/messages")]
    public async Task<IActionResult> AddMessageToTicket(Guid id, AddMessageToTicketCommand command)
    {
        command = command with { TicketId = id };
        var message = await _mediator.Send(command);
        if (message == null) return NotFound();
        return Ok(message);
    }
}
