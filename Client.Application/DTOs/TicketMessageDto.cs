using System;
using System.Collections.Generic;

namespace Client.Application.DTOs;

public class TicketMessageDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsInternal { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public List<AttachmentDto> Attachments { get; set; } = new();
}