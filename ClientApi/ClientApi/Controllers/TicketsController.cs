using Client.Application.Feature.Tickets.Commands.AddMessage;
using Client.Application.Feature.Tickets.Commands.Create;
using Client.Application.Feature.Tickets.Queries.GetClientTicketById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Infrastructure.Extensions;

namespace ClientApi.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TicketsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET /api/tickets/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyTickets([FromQuery] GetClientTicketsQueryDto dto)
        {
            var query = new GetClientTicketsQuery
            {
                ClientId = User.GetUserId(),
                Status = dto.Status,
                Priority = dto.Priority,
                Page = dto.Page ?? 1,
                PageSize = dto.PageSize ?? 10
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // POST /api/tickets
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketCommand command)
        {
            command = command with { ClientId = User.GetUserId() };

            var ticket = await _mediator.Send(command);

            return CreatedAtAction(
                nameof(GetClientTicketByIdQuery),
                new { id = ticket.Id },
                ticket
            );
        }

        // POST /api/tickets/{id}/messages
        [HttpPost("{id:guid}/messages")]
        public async Task<IActionResult> AddMessageToTicket(
            Guid id,
            [FromBody] AddMessageToTicketCommand command)
        {
            command = command with
            {
                TicketId = id,
                AuthorId = User.GetUserId()
            };

            var message = await _mediator.Send(command);
            return Ok(message);
        }
    }
}


// GET /api/tickets/{id} pentru TechSupport
//[HttpGet("{id:guid}")]
//public async Task<IActionResult> GetTicketById(Guid id)
//{
//    var query = new GetClientTicketByIdQuery(new GetClientTicketByIdQueryDto
//    {
//        TicketId = id,
//        ClientId = User.GetUserId()
//    });

//    var result = await _mediator.Send(query);
//    return Ok(result);
//}