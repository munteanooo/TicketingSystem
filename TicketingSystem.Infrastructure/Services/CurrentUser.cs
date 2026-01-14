using System.Security.Claims;
using Client.Application.Contracts.Services;
using Microsoft.AspNetCore.Http;

namespace TicketingSystem.Infrastructure.Services
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                return Guid.TryParse(userIdClaim, out var id) ? id : null;
            }
        }

        public string? Email => _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Email)?.Value;

        public string? FullName
        {
            get
            {
                var fullNameClaim = _httpContextAccessor.HttpContext?.User?
                    .FindFirst("fullName")?.Value;

                if (!string.IsNullOrEmpty(fullNameClaim))
                    return fullNameClaim;

                var firstName = _httpContextAccessor.HttpContext?.User?
                    .FindFirst(ClaimTypes.GivenName)?.Value;
                var lastName = _httpContextAccessor.HttpContext?.User?
                    .FindFirst(ClaimTypes.Surname)?.Value;

                return $"{firstName} {lastName}".Trim();
            }
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?
            .Identity?.IsAuthenticated ?? false;

        public bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User?
                .IsInRole(role) ?? false;
        }

        public bool HasClaim(string claimType, string claimValue)
        {
            return _httpContextAccessor.HttpContext?.User?
                .HasClaim(claimType, claimValue) ?? false;
        }
    }
}
