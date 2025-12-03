using System.ComponentModel.DataAnnotations;
using ClientApi.Models;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.ClientApi.Models
{
    public class TicketDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string? AssignedToName { get; set; }
        public int MessageCount { get; set; }
    }

    public class TicketDetailDto : TicketDto
    {
        public string ClosingNotes { get; set; } = string.Empty;
        public UserDto? Client { get; set; }
        public UserDto? AssignedTo { get; set; }
        public List<MessageDto> Messages { get; set; } = new();
    }

    public class CreateTicketRequest
    {
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;
        [Required] public TicketPriority Priority { get; set; }
        public string? InitialMessage { get; set; }
    }

    public class UpdateTicketRequest
    {
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;
        [Required] public TicketPriority Priority { get; set; }
        [Required] public TicketStatus Status { get; set; }
        public int? AssignedToId { get; set; }
        public string? ClosingNotes { get; set; }
    }

    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsInternal { get; set; }
        public UserDto? User { get; set; }
        public List<AttachmentDto> Attachments { get; set; } = new();
    }

    public class AddMessageRequest
    {
        [Required] public string Content { get; set; } = string.Empty;
        public bool IsInternal { get; set; }
        public List<IFormFile>? Attachments { get; set; } = new();
    }

    public class AttachmentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public string FilePath { get; set; } = string.Empty;
    }

    public class StatsDto
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int UnassignedTickets { get; set; }
        public int MyAssignedTickets { get; set; }
    }
}