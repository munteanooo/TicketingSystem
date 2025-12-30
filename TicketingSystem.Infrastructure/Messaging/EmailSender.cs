using System.Net;
using System.Net.Mail;
using Client.Application.Contracts.Messaging;

namespace TicketingSystem.Infrastructure.Messaging
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _emailAddress;
        private readonly string _emailPassword;

        public EmailSender(string smtpServer, int smtpPort, string emailAddress, string emailPassword)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _emailAddress = emailAddress;
            _emailPassword = emailPassword;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await SendEmailAsync(to, subject, body, null);
        }

        public async Task SendEmailAsync(string to, string subject, string body, string? htmlBody)
        {
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_emailAddress, _emailPassword);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailAddress),
                    Subject = subject,
                    Body = htmlBody ?? body,
                    IsBodyHtml = htmlBody != null
                };

                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}