using Chronoria_ConsumerWorkers.utils;

namespace Chronoria_ConsumerWorkers.Services
{
    public class CanceledReceiptEmailService : ICanceledReceiptEmailService
    {
        private readonly IEmailTemplateService emailTemplateService;
        private readonly ICoreEmailService coreEmailService;

        public CanceledReceiptEmailService(
            IEmailTemplateService emailTemplateService,
            ICoreEmailService coreEmailService)
        {
            this.emailTemplateService = emailTemplateService;
            this.coreEmailService = coreEmailService;
        }

        public async Task SendCanceledReceiptEmail(string email, string recipientName, string recipientEmail, long sendTime)
        {
            string sendTimeString = TimeUtils.DateTimeToString(sendTime);

            string htmlContent = await emailTemplateService.GetEmailTemplate("CanceledReceipt");
            htmlContent = htmlContent.Replace("[RecipientName]", recipientName);
            htmlContent = htmlContent.Replace("[RecipientEmail]", recipientEmail);
            htmlContent = htmlContent.Replace("[SendTime]", sendTimeString);

            string? subject = HtmlUtils.GetTitle(htmlContent);
            if (subject == null)
            {
                throw new ArgumentNullException("Cannot extract email subject from template");
            }

            await coreEmailService.SendHtml(email, email, subject, htmlContent);
        }
    }
}
