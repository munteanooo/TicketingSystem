using System;
using TicketingSystem.Domain.Enums;

namespace Client.Application.DTOs;

public class TicketDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? ClosingNotes { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
}