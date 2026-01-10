//namespace TechApi.Controllers
//{
//    using System;
//    using System.Threading.Tasks;
//    using Tech.Application.Feature.Tickets.Commands.Delete;
//    using Tech.Application.Feature.Tickets.Commands.UpdateStatus;
//    using MediatR;
//    using Microsoft.AspNetCore.Authorization;
//    using Microsoft.AspNetCore.Mvc;

//    [ApiController]
//    [Route("api/admin/[controller]")]
//    [Authorize(Roles = "Admin")]
//    public class AdminTicketsController : ControllerBase
//    {
//        private readonly IMediator _mediator;

//        public AdminTicketsController(IMediator mediator)
//        {
//            _mediator = mediator;
//        }

//        [HttpPut("{id:guid}/status")]
//        public async Task<IActionResult> UpdateTicketStatus(Guid id, UpdateTicketStatusCommand command)
//        {
//            try
//            {
//                command.TicketId = id;
//                var updatedTicket = await _mediator.Send(command);
//                if (updatedTicket == null) return NotFound();
//                return Ok(updatedTicket);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
//            }
//        }

//        [HttpDelete("{id:guid}")]
//        public async Task<IActionResult> DeleteTicket(Guid id)
//        {
//            try
//            {
//                var result = await _mediator.Send(new DeleteTicketCommand(id));
//                if (!result) return NotFound();
//                return NoContent();
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
//            }
//        }
//    }
//}