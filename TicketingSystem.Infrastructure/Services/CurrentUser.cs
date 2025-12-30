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

        public int? UserId
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User
                    .FindFirst(ClaimTypes.NameIdentifier);
                return int.TryParse(claim?.Value, out var userId) ? userId : null;
            }
        }

        public string? Email
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User
                    .FindFirst(ClaimTypes.Email)?.Value;
            }
        }

        public string? FullName
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User
                    .FindFirst(ClaimTypes.Name)?.Value;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
            }
        }
    }
}