using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Application.Contracts.Interfaces;

namespace TicketingSystem.API.Controllers
{
    /// <summary>
    /// Users API endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Get all technicians
        /// </summary>
        [HttpGet("technicians")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTechnicians(CancellationToken cancellationToken)
        {
            var technicians = await _userRepository.GetAllTechniciansAsync(cancellationToken);
            return Ok(technicians);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                return NotFound();
            return Ok(user);
        }
    }
}