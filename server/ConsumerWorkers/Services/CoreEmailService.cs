using SendGrid;
using SendGrid.Helpers.Mail;

namespace Chronoria_ConsumerWorkers.Services
{
    public class CoreEmailService : ICoreEmailService
    {
        private readonly SendGridClient client;
        private readonly string mainEmail;
        private readonly string mainName;

        public CoreEmailService(
            string apiKey,
            string mainEmail,
            string mainName)
        {
            client = new SendGridClient(apiKey);            // bad pattern? (should init class from injection instead)
            this.mainEmail = mainEmail;
            this.mainName = mainName;
        }

        public async Task SendPlainText(string senderEmail, string senderName, string recipientEmail, string recipientName, string subject, string content)
        {
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(senderEmail, senderName),
                Subject = subject,
                PlainTextContent = content
            };
            msg.AddTo(new EmailAddress(recipientEmail, recipientName));
            var response = await client.SendEmailAsync(msg);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Email fails to be delivered to " + recipientEmail + "; error: " + response.ToString());
            }
        }

        public async Task SendPlainText(string recipientEmail, string recipientName, string subject, string content)
        {
            await SendPlainText(mainEmail, mainName, recipientEmail, recipientName, subject, content);
        }
    }
}
