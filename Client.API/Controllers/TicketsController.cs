using MediatR;
using Microsoft.AspNetCore.Mvc;
using Client.Application.Commands;
using Client.Application.Queries;
using Client.Application.DTOs;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateTicket(CreateTicketCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("client/{clientId}")]
    public async Task<ActionResult<List<TicketDto>>> GetClientTickets(int clientId)
    {
        var query = new GetClientTicketsQuery { ClientId = clientId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}