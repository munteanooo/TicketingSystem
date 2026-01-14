using Client.Application.Contracts.Services;
using Client.Application.Feature.Tickets.Commands.AddMessage;
using Client.Application.Feature.Tickets.Commands.Create;
using Client.Application.Feature.Tickets.Commands.Reopen;
using Client.Application.Feature.Tickets.Commands.Ticket;
using Client.Application.Feature.Tickets.Queries.GetClientTicketById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Domain.Enums;

namespace ClientApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUser _currentUser;

        public TicketsController(IMediator mediator, ICurrentUser currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Create a new ticket
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketCommandDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var clientId = _currentUser.UserId;
                if (clientId == null)
                    return Unauthorized(new { message = "User not authenticated" });

                var command = new CreateTicketCommand(
                    clientId.Value,
                    dto.Title,
                    dto.Description,
                    dto.Category,
                    dto.Priority
                );

                var response = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetTicketById), new { ticketId = response.Title }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating ticket", details = ex.Message });
            }
        }

        /// <summary>
        /// Get all tickets for current user with optional filters
        /// </summary>
        [HttpGet("my-tickets")]
        public async Task<IActionResult> GetMyTickets(
            [FromQuery] TicketStatus? status = null,
            [FromQuery] TicketPriority? priority = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var clientId = _currentUser.UserId;
                if (clientId == null)
                    return Unauthorized(new { message = "User not authenticated" });

                var query = new GetClientTicketsQuery
                {
                    ClientId = clientId.Value,
                    Status = status,
                    Priority = priority,
                    Page = page,
                    PageSize = pageSize
                };

                var response = await _mediator.Send(query);

                return Ok(new
                {
                    success = true,
                    data = response,
                    count = response.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching tickets", details = ex.Message });
            }
        }

        /// <summary>
        /// Get ticket by ID
        /// </summary>
        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetTicketById([FromRoute] Guid ticketId)
        {
            try
            {
                var clientId = _currentUser.UserId;
                if (clientId == null)
                    return Unauthorized(new { message = "User not authenticated" });

                var query = new GetClientTicketByIdQuery(new GetClientTicketByIdQueryDto
                {
                    TicketId = ticketId,
                    ClientId = clientId.Value
                });

                var response = await _mediator.Send(query);

                if (response == null || response.Id == Guid.Empty)
                    return NotFound(new { message = "Ticket not found" });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching ticket", details = ex.Message });
            }
        }

        /// <summary>
        /// Update ticket details
        /// </summary>
        [HttpPut("{ticketId}")]
        public async Task<IActionResult> UpdateTicket(
            [FromRoute] Guid ticketId,
            [FromBody] TicketCommandDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var clientId = _currentUser.UserId;
                if (clientId == null)
                    return Unauthorized(new { message = "User not authenticated" });

                var command = new TicketCommand(new TicketCommandDto
                {
                    Id = ticketId,
                    ClientId = clientId.Value,
                    Title = dto.Title ?? string.Empty,
                    Description = dto.Description ?? string.Empty,
                    Priority = dto.Priority ?? string.Empty,
                    Status = dto.Status ?? string.Empty,
                    Category = dto.Category ?? string.Empty
                });

                var response = await _mediator.Send(command);

                if (response == null || response.Id == Guid.Empty)
                    return BadRequest(new { message = "Failed to update ticket" });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating ticket", details = ex.Message });
            }
        }

        /// <summary>
        /// Reopen a closed ticket
        /// </summary>
        [HttpPost("{ticketId}/reopen")]
        public async Task<IActionResult> ReopenTicket(
            [FromRoute] Guid ticketId,
            [FromBody] ReopenTicketCommandDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var clientId = _currentUser.UserId;
                if (clientId == null)
                    return Unauthorized(new { message = "User not authenticated" });

                var command = new ReopenTicketCommand(new ReopenTicketCommandDto
                {
                    TicketId = ticketId,
                    Reason = dto.Reason
                });

                var response = await _mediator.Send(command);

                if (response == null || response.Id == Guid.Empty)
                    return BadRequest(new { message = "Failed to reopen ticket" });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error reopening ticket", details = ex.Message });
            }
        }

        /// <summary>
        /// Add message to ticket
        /// </summary>
        [HttpPost("{ticketId}/messages")]
        public async Task<IActionResult> AddMessageToTicket(
            [FromRoute] Guid ticketId,
            [FromBody] AddMessageToTicketCommandDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = _currentUser.UserId;
                if (userId == null)
                    return Unauthorized(new { message = "User not authenticated" });

                var command = new AddMessageToTicketCommand(
                    ticketId,
                    userId.Value,
                    _currentUser.FullName ?? "Unknown",
                    dto.Content
                );

                var response = await _mediator.Send(command);

                if (response == null || response.Id == Guid.Empty)
                    return BadRequest(new { message = "Failed to add message" });

                return CreatedAtAction(nameof(GetTicketMessages), new { ticketId }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error adding message", details = ex.Message });
            }
        }

        /// <summary>
        /// Get all messages for a ticket
        /// </summary>
        [HttpGet("{ticketId}/messages")]
        public async Task<IActionResult> GetTicketMessages([FromRoute] Guid ticketId)
        {
            try
            {
                var userId = _currentUser.UserId;
                if (userId == null)
                    return Unauthorized(new { message = "User not authenticated" });

                var query = new GetTicketMessagesQuery(ticketId, userId.Value, false);
                var response = await _mediator.Send(query);

                if (response == null)
                    return NotFound(new { message = "Messages not found" });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching messages", details = ex.Message });
            }
        }

        /// <summary>
        /// Close a ticket
        /// </summary>
        [HttpPost("{ticketId}/close")]
        public async Task<IActionResult> CloseTicket([FromRoute] Guid ticketId)
        {
            try
            {
                var clientId = _currentUser.UserId;
                if (clientId == null)
                    return Unauthorized(new { message = "User not authenticated" });

                var command = new TicketCommand(new TicketCommandDto
                {
                    Id = ticketId,
                    ClientId = clientId.Value,
                    Status = TicketStatus.Closed.ToString()
                });

                var response = await _mediator.Send(command);

                if (response == null || response.Id == Guid.Empty)
                    return BadRequest(new { message = "Failed to close ticket" });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error closing ticket", details = ex.Message });
            }
        }
    }
}
