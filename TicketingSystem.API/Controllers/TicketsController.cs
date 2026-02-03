using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Application.Tickets.Commands.AddMessage;
using TicketingSystem.Application.Tickets.Commands.AssignTicket;
using TicketingSystem.Application.Tickets.Commands.ChangeStatus;
using TicketingSystem.Application.Tickets.Commands.CloseTicket;
using TicketingSystem.Application.Tickets.Commands.CreateTicket;
using TicketingSystem.Application.Tickets.Queries.GetClientTickets;
using TicketingSystem.Application.Tickets.Queries.GetTechTickets;
using TicketingSystem.Application.Tickets.Queries.GetTicketDetails;
using TicketingSystem.Application.Tickets.Queries.GetTicketMessages;

namespace TicketingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TicketsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // --- DTO-uri locale pentru Request-uri simple ---
        public class MessageRequest { public string Content { get; set; } = string.Empty; }

        // --- Endpoint-uri ---

        [HttpPost]
        [ProducesResponseType(typeof(CreateTicketCommandResponseDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketCommandDto requestDto, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();

            // Injectăm ClientId din Token în DTO
            requestDto.ClientId = userId;

            // Împachetăm DTO-ul în Comandă folosind numele corect al parametrului: CommandDto
            var command = new CreateTicketCommand(requestDto);

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetTicketById), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTicketById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetTicketDetailsQuery(id), cancellationToken);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost("{id:guid}/messages")]
        public async Task<IActionResult> AddMessage(Guid id, [FromBody] MessageRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var dto = new AddMessageCommandDto
            {
                TicketId = id,
                AuthorId = userId,
                Content = request.Content
            };

            var result = await _mediator.Send(new AddMessageCommand(dto), cancellationToken);
            return CreatedAtAction(nameof(GetTicketMessages), new { id }, result);
        }

        [HttpGet("{id:guid}/messages")]
        public async Task<IActionResult> GetTicketMessages(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetTicketMessagesQuery(id), cancellationToken);
            return Ok(result);
        }

        [HttpGet("my-tickets")]
        public async Task<IActionResult> GetMyTickets(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            var result = await _mediator.Send(new GetClientTicketsQuery(userId), cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}/assign")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignTicket(Guid id, [FromBody] AssignTicketCommandDto dto, CancellationToken cancellationToken)
        {
            dto.TicketId = id;
            var result = await _mediator.Send(new AssignTicketCommand(dto), cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "Admin,Technician")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeStatusCommandDto dto, CancellationToken cancellationToken)
        {
            dto.TicketId = id;
            var result = await _mediator.Send(new ChangeStatusCommand(dto), cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}/close")]
        public async Task<IActionResult> CloseTicket(Guid id, [FromBody] CloseTicketCommandDto dto, CancellationToken cancellationToken)
        {
            dto.TicketId = id;
            var result = await _mediator.Send(new CloseTicketCommand(dto), cancellationToken);
            return Ok(result);
        }

        // --- Helper Methods ---

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? User.FindFirst("sub")?.Value;

            return Guid.TryParse(userIdClaim, out var guid) ? guid : Guid.Empty;
        }
    }
}