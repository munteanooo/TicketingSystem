using MediatR;
using Client.Application.Feature.Tickets.Commands.Login;

namespace Client.Application.Feature.Tickets.Commands.Register
{
    public class RegisterCommand : IRequest<LoginCommandResponseDto>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
