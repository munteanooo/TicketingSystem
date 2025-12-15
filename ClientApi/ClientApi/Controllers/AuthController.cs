namespace ClientApi.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;
    using System.Security.Claims;
    using System.Text;
    using TicketingSystem.Infrastructure.Identity;
    using TicketingSystem.Domain.Entities;
    using TicketingSystem.Infrastructure.Data;
    using ClientApi.Models;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.Email,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Registration failed: " + string.Join(", ", result.Errors)
                });

            await _userManager.AddToRoleAsync(user, "Client");

            var domainUser = new DomainUser
            {
                Id = Guid.NewGuid(),
                IdentityUserId = user.Id,
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Role = TicketingSystem.Domain.Enums.UserRole.Client,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.DomainUsers.Add(domainUser);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user, domainUser);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Registration successful",
                Token = token,
                User = new UserInfo
                {
                    Id = domainUser.Id,
                    Email = user.Email,
                    FullName = domainUser.FullName,
                    PhoneNumber = domainUser.PhoneNumber,
                    Role = "Client"
                }
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "Invalid email or password"
                });

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "Invalid email or password"
                });

            var domainUser = _context.DomainUsers.FirstOrDefault(du => du.IdentityUserId == user.Id);

            if (domainUser == null || !domainUser.IsActive)
                return Unauthorized(new AuthResponse
                {
                    Success = false,
                    Message = "User account is not active"
                });

            var token = GenerateJwtToken(user, domainUser);

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = new UserInfo
                {
                    Id = domainUser.Id,
                    Email = user.Email,
                    FullName = domainUser.FullName,
                    PhoneNumber = domainUser.PhoneNumber,
                    Role = domainUser.Role.ToString()
                }
            });
        }

        private string GenerateJwtToken(ApplicationUser user, DomainUser domainUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, domainUser.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, domainUser.FullName),
                    new Claim("IdentityUserId", user.Id.ToString()),
                    new Claim(ClaimTypes.Role, domainUser.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}