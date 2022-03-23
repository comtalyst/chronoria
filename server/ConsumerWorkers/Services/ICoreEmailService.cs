namespace Chronoria_ConsumerWorkers.Services
{
    public interface ICoreEmailService
    {
        public Task SendPlainText(
            string senderEmail,
            string senderName,
            string recipientEmail,
            string recipientName,
            string subject,
            string content);
        // use default email
        public Task SendPlainText(
            string recipientEmail,
            string recipientName,
            string subject,
            string content);
    }
}
