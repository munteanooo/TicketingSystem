using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TicketingSystem.Application.Contracts.Interfaces;

namespace TicketingSystem.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;

        public string? FullName => User?.FindFirst(ClaimTypes.Name)?.Value
                                   ?? User?.FindFirst("name")?.Value;

        public string? UserRole => User?.FindFirst(ClaimTypes.Role)?.Value;

        public bool IsClient => UserRole == "Client";
        public bool IsTechnician => UserRole == "Technician";
        public bool IsAdmin => UserRole == "Admin";
        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
    }
}