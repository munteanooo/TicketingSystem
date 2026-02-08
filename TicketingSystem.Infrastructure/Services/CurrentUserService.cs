using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TicketingSystem.Application.Contracts.Interfaces;

namespace TicketingSystem.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User?.FindFirst("sub")?.Value;
        public string? UserRole => User?.FindFirst(ClaimTypes.Role)?.Value?.Trim();

        public bool IsAdmin => string.Equals(UserRole, "Admin", StringComparison.OrdinalIgnoreCase);
        public bool IsTechnician => string.Equals(UserRole, "TechSupport", StringComparison.OrdinalIgnoreCase);

        public bool IsStaff => IsAdmin || IsTechnician;

        public bool IsClient => string.Equals(UserRole, "Client", StringComparison.OrdinalIgnoreCase);
        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;
        public string? FullName => User?.FindFirst(ClaimTypes.Name)?.Value ?? User?.FindFirst("name")?.Value;
    }
}