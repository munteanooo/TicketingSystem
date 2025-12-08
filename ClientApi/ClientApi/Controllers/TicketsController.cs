using Microsoft.AspNetCore.Mvc;
using ClientApi.Models;
using System.Collections.Generic;

namespace ClientApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private static List<TicketResponse> _tickets = new List<TicketResponse>();
        private static int _nextId = 1;

        [HttpPost]
        public IActionResult CreateTicket([FromBody] CreateTicketRequest request)
        {
            // SIMPLU: salvează în memorie
            var ticket = new TicketResponse
            {
                Id = _nextId++,
                Title = request.Title,
                Status = "Open"
            };

            _tickets.Add(ticket);

            return Ok(ticket);
        }

        [HttpGet]
        public IActionResult GetAllTickets()
        {
            // SIMPLU: returnează toate ticketele
            return Ok(_tickets);
        }

        [HttpGet("{id}")]
        public IActionResult GetTicket(int id)
        {
            // SIMPLU: găsește ticket după ID
            var ticket = _tickets.Find(t => t.Id == id);

            if (ticket == null)
                return NotFound();

            return Ok(ticket);
        }
    }
}