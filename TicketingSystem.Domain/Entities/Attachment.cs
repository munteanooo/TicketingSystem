using System;
using System.Collections.Generic;

namespace TicketingSystem.Domain.Entities
{
    public class Attachment
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public int TicketMessageId { get; set; }
        public TicketMessage TicketMessage { get; set; } = null!;

        public Attachment() { }

        public Attachment(string fileName, string filePath, string contentType, long fileSize, int ticketMessageId)
        {
            FileName = fileName;
            FilePath = filePath;
            ContentType = contentType;
            FileSize = fileSize;
            TicketMessageId = ticketMessageId;
        }
    }
}