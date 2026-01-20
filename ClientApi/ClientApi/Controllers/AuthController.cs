using Client.Application.Feature.Auth.Login;
using Client.Application.Feature.Auth.Register;
using MediatR;
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
        public async Task<IActionResult> Register([FromBody] RegisterCommandDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
        public async Task<IActionResult> Login([FromBody] LoginCommandDto dto)
        {
            Console.WriteLine($"API LOGIN DTO: {dto?.Email} / {dto?.Password}");
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Model invalid", errors = ModelState });

            try
            {
                var command = new LoginCommand(dto);
                var response = await _mediator.Send(command);

                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "A apărut o eroare la login",
                    details = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var authService = HttpContext.RequestServices.GetService(typeof(Client.Application.Contracts.Services.IAuthService))
                    as Client.Application.Contracts.Services.IAuthService;

                if (authService == null)
                    return StatusCode(500, new { message = "Auth service not found" });

                var response = await authService.RefreshTokenAsync(request.RefreshToken);

                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "A apărut o eroare la refresh token",
                    details = ex.Message
                });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            try
            {
                var authService = HttpContext.RequestServices.GetService(typeof(Client.Application.Contracts.Services.IAuthService))
                    as Client.Application.Contracts.Services.IAuthService;

                if (authService == null)
                    return StatusCode(500, new { message = "Auth service not found" });

                await authService.LogoutAsync(request.UserId);

                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "A apărut o eroare la logout",
                    details = ex.Message
                });
            }
        }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class LogoutRequest
    {
        public Guid UserId { get; set; }
    }
}
