using TicketingSystem.Domain.Enums;

namespace Client.Application.Contracts.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Client; 
        public bool IsActive { get; set; } = true;
        public string? PhoneNumber { get; set; }

        public string RoleString => Role.ToString();
        public string[] Roles => new[] { Role.ToString() };
    }
}