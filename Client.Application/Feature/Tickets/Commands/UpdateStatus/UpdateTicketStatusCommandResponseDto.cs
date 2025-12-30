// Command DTO
using MediatR;

namespace Client.Application.Feature.Tickets.Commands.UpdateStatus
{
    public class UpdateTicketStatusCommandResponseDto
    {
        public string NewStatus { get; set; } = string.Empty;
        public string? Message { get; set; }
        public Guid UserId { get; set; }
    }
}
