namespace Client.Application.Contracts.Messaging
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailAsync(string to, string subject, string body, string? htmlBody);
    }
}