using System;

namespace TicketingSystem.Domain.Entities
{
    public class Attachment
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public int TicketMessageId { get; set; }
        public TicketMessage TicketMessage { get; set; }

        public Attachment() { }

        public Attachment(string fileName, string filePath, int ticketMessageId)
        {
            FileName = fileName;
            FilePath = filePath;
            TicketMessageId = ticketMessageId;
        }
    }
}