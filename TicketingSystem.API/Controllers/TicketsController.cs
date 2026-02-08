using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Application.Tickets.Commands.AddMessage;
using TicketingSystem.Application.Tickets.Commands.AssignTicket;
using TicketingSystem.Application.Tickets.Commands.CloseTicket;
using TicketingSystem.Application.Tickets.Commands.CreateTicket;
using TicketingSystem.Application.Tickets.Queries;
using TicketingSystem.Application.Tickets.Queries.GetClientTickets;
using TicketingSystem.Application.Tickets.Queries.GetTicketDetails;

namespace TicketingSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;
    public TicketsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("my-tickets")]
    public async Task<IActionResult> GetMyTickets(CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();
        return Ok(await _mediator.Send(new GetClientTicketsQuery(userId), ct));
    }

    [HttpGet("unassigned")]
    [Authorize(Roles = "Admin,TechSupport")]
    public async Task<IActionResult> GetUnassignedTickets(CancellationToken ct)
    {
        return Ok(await _mediator.Send(new GetUnassignedTicketsQuery(), ct));
    }

    // REZOLVĂ 404: Endpoint-ul pentru tichetele preluate de tehnician
    [HttpGet("my-assigned")]
    [Authorize(Roles = "Admin,TechSupport")]
    public async Task<IActionResult> GetMyAssignedTickets(CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();
        return Ok(await _mediator.Send(new GetMyAssignedTicketsQuery(userId), ct));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTicketById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTicketDetailsQuery(id), ct);
        return result != null ? Ok(result) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketCommandDto dto, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();
        dto.ClientId = userId;
        var result = await _mediator.Send(new CreateTicketCommand(dto), ct);
        return CreatedAtAction(nameof(GetTicketById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}/assign")]
    [Authorize(Roles = "Admin,TechSupport")]
    public async Task<IActionResult> AssignTicket(Guid id, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();
        var result = await _mediator.Send(new AssignTicketCommand(new AssignTicketCommandDto { TicketId = id, TechnicianId = userId }), ct);
        return Ok(result);
    }

    [HttpPut("{id:guid}/close")]
    [Authorize(Roles = "Admin,TechSupport")]
    public async Task<IActionResult> CloseTicket(Guid id, [FromBody] CloseTicketCommandDto dto, CancellationToken ct)
    {
        dto.TicketId = id; 

        var command = new CloseTicketCommand(dto);

        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/messages")]
    public async Task<IActionResult> AddMessage(Guid id, [FromBody] AddMessageCommandDto dto, CancellationToken ct)
    {
        dto.TicketId = id;
        return Ok(await _mediator.Send(new AddMessageCommand(dto), ct));
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        return claim != null && Guid.TryParse(claim.Value, out var id) ? id : Guid.Empty;
    }
}