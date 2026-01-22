using Client.Application.Feature.Auth.Login;
using Client.Application.Feature.Auth.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterCommandDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Model invalid", errors = ModelState });

            try
            {
                var command = new RegisterCommand(dto);
                var response = await _mediator.Send(command);

                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "A apărut o eroare la înregistrare",
                    details = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommandDto dto)
        {
            Console.WriteLine($"🔐 API LOGIN DTO: {dto?.Email} / {dto?.Password}");

            if (!ModelState.IsValid)
                return BadRequest(new { message = "Model invalid", errors = ModelState });

            try
            {
                var command = new LoginCommand(dto);
                var response = await _mediator.Send(command);

                if (!response.Success)
                {
                    Console.WriteLine($"❌ Login failed: {response.Message}");
                    return Unauthorized(response);
                }

                // 🔑 Mapează răspunsul handler-ului la LoginResponse
                var loginResponse = new LoginResponse
                {
                    Success = response.Success,
                    Message = response.Message,
                    Token = response.Token ?? string.Empty,
                    RefreshToken = response.RefreshToken ?? string.Empty,
                    UserId = response.User?.Id ?? Guid.Empty,
                    Email = response.User?.Email ?? string.Empty,
                    FullName = response.User?.FullName ?? string.Empty
                };

                Console.WriteLine($"✅ Login successful: {response.Email}");
                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Login exception: {ex.Message}");
                return StatusCode(500, new
                {
                    message = "A apărut o eroare la login",
                    details = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        [HttpPost("refresh")]
        [Authorize]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Model invalid", errors = ModelState });

            try
            {
                var authService = HttpContext.RequestServices.GetService(typeof(Client.Application.Contracts.Services.IAuthService))
                    as Client.Application.Contracts.Services.IAuthService;

                if (authService == null)
                    return StatusCode(500, new { message = "Auth service not found" });

                var response = await authService.RefreshTokenAsync(request.RefreshToken);

                if (!response.Success)
                    return Unauthorized(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Refresh token exception: {ex.Message}");
                return StatusCode(500, new
                {
                    message = "A apărut o eroare la refresh token",
                    details = ex.Message
                });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            try
            {
                var authService = HttpContext.RequestServices.GetService(typeof(Client.Application.Contracts.Services.IAuthService))
                    as Client.Application.Contracts.Services.IAuthService;

                if (authService == null)
                    return StatusCode(500, new { message = "Auth service not found" });

                await authService.LogoutAsync(request.UserId);

                Console.WriteLine($"🚪 User {request.UserId} logged out");
                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Logout exception: {ex.Message}");
                return StatusCode(500, new
                {
                    message = "A apărut o eroare la logout",
                    details = ex.Message
                });
            }
        }
    }

    // 📋 DTO pentru login response
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    // 📋 DTO pentru refresh request
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    // 📋 DTO pentru logout request
    public class LogoutRequest
    {
        public Guid UserId { get; set; }
    }
}