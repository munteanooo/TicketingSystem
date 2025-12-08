using Microsoft.AspNetCore.Mvc;
using ClientApi.Models;

namespace ClientApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // SIMPLU: verifică dacă email și parolă sunt "admin"
            if (request.Email == "admin@test.com" && request.Password == "admin123")
            {
                return Ok(new AuthResponse
                {
                    Token = "fake-jwt-token-123",
                    Message = "Login successful"
                });
            }

            return Unauthorized(new AuthResponse
            {
                Message = "Wrong email or password"
            });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            // SIMPLU: mereu acceptă înregistrarea
            return Ok(new AuthResponse
            {
                Token = "fake-jwt-token-new-user",
                Message = "Registration successful"
            });
        }
    }
}