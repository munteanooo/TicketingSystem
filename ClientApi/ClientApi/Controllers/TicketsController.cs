namespace ClientApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Client.Application.Commands;
    using Client.Application.Queries;
    using ClientApi.Models;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TicketingSystem.Application.Commands;
    using TicketingSystem.Application.DTOs;

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

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdClaim?.Value ?? Guid.Empty.ToString());
        }

        [HttpPost]
        public async Task<ActionResult<TicketResponse>> CreateTicket([FromBody] CreateTicketRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();

            var command = new CreateTicketCommand
            {
                ClientId = userId,
                Ticket = new CreateTicketDto
                {
                    Title = request.Title,
                    Description = request.Description,
                    Category = request.Category,
                    Priority = request.Priority
                }
            };

            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetTicketById), new { id = result.Id }, MapToResponse(result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDetailsResponse>> GetTicketById(Guid id)
        {
            var query = new GetTicketByIdQuery { TicketId = id };
            var result = await _mediator.Send(query);

            var userId = GetUserId();
            if (result.ClientId != userId)
                return Forbid();

            return Ok(MapToDetailsResponse(result));
        }

        [HttpGet]
        public async Task<ActionResult<List<TicketResponse>>> GetMyTickets()
        {
            var userId = GetUserId();
            var query = new GetClientTicketsQuery { ClientId = userId };
            var result = await _mediator.Send(query);

            return Ok(result.ConvertAll(t => MapToResponse(t)));
        }

        [HttpPost("{id}/messages")]
        public async Task<ActionResult<TicketMessageResponse>> AddMessage(Guid id, [FromBody] AddMessageRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();

            var ticketQuery = new GetTicketByIdQuery { TicketId = id };
            var ticket = await _mediator.Send(ticketQuery);

            if (ticket.ClientId != userId)
                return Forbid();

            var command = new AddMessageToTicketCommand
            {
                TicketId = id,
                AuthorId = userId,
                Content = request.Content
            };

            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetTicketById), new { id }, MapMessageToResponse(result));
        }

        [HttpGet("{id}/messages")]
        public async Task<ActionResult<List<TicketMessageResponse>>> GetTicketMessages(Guid id)
        {
            var userId = GetUserId();

            var ticketQuery = new GetTicketByIdQuery { TicketId = id };
            var ticket = await _mediator.Send(ticketQuery);

            if (ticket.ClientId != userId)
                return Forbid();

            var query = new GetTicketMessagesQuery { TicketId = id };
            var result = await _mediator.Send(query);

            return Ok(result.ConvertAll(m => MapMessageToResponse(m)));
        }

        [HttpPost("{id}/reopen")]
        public async Task<ActionResult<TicketResponse>> ReopenTicket(Guid id, [FromBody] UpdateTicketStatusRequest request)
        {
            var userId = GetUserId();

            var ticketQuery = new GetTicketByIdQuery { TicketId = id };
            var ticket = await _mediator.Send(ticketQuery);

            if (ticket.ClientId != userId)
                return Forbid();

            var command = new ReopenTicketCommand
            {
                TicketId = id,
                Reason = request.Message
            };

            var result = await _mediator.Send(command);

            return Ok(MapToResponse(result));
        }

        private TicketResponse MapToResponse(TicketDto dto)
        {
            return new TicketResponse
            {
                Id = dto.Id,
                TicketNumber = dto.TicketNumber,
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                Status = dto.Status,
                Category = dto.Category,
                ClientId = dto.ClientId,
                ClientName = dto.ClientName,
                AssignedToAgentId = dto.AssignedToAgentId,
                AssignedToAgentName = dto.AssignedToAgentName,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                ResolvedAt = dto.ResolvedAt
            };
        }

        private TicketDetailsResponse MapToDetailsResponse(TicketDetailsDto dto)
        {
            return new TicketDetailsResponse
            {
                Id = dto.Id,
                TicketNumber = dto.TicketNumber,
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                Status = dto.Status,
                Category = dto.Category,
                ClientId = dto.ClientId,
                ClientName = dto.ClientName,
                ClientEmail = dto.ClientEmail,
                AssignedToAgentId = dto.AssignedToAgentId,
                AssignedToAgentName = dto.AssignedToAgentName,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                ResolvedAt = dto.ResolvedAt,
                Messages = dto.Messages.ConvertAll(m => MapMessageToResponse(m))
            };
        }

        private TicketMessageResponse MapMessageToResponse(TicketMessageDto dto)
        {
            return new TicketMessageResponse
            {
                Id = dto.Id,
                TicketId = dto.TicketId,
                AuthorId = dto.AuthorId,
                AuthorName = dto.AuthorName,
                Content = dto.Content,
                CreatedAt = dto.CreatedAt
            };
        }
    }
}