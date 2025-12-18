using Microsoft.AspNetCore.Mvc;
using Client.Application.DTOs;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Domain.Interfaces;
using TicketingSystem.Infrastructure.Services;

namespace TicketingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;

        public AuthController(IUnitOfWork unitOfWork, IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and password are required");

            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password");

            if (!user.IsActive)
                return Unauthorized("User account is inactive");

            var token = _jwtService.GenerateToken(user);
            var response = new LoginResponse
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString()
                }
            };
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName))
                return BadRequest("All fields are required");

            var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (existingUser != null)
                return BadRequest("Email already exists");

            var user = new ApplicationUser(
                email: request.Email,
                firstName: request.FirstName,
                lastName: request.LastName,
                role: TicketingSystem.Domain.Enums.UserRole.Client
            )
            {
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsActive = true
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { message = "User registered successfully. Please log in." });
        }
    }
}