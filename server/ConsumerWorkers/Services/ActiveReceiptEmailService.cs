using Chronoria_ConsumerWorkers.utils;

namespace Chronoria_ConsumerWorkers.Services
{
    public class ActiveReceiptEmailService : IActiveReceiptEmailService
    {
        private readonly IFrontLinkService frontLinkService;
        private readonly IEmailTemplateService emailTemplateService;
        private readonly ICoreEmailService coreEmailService;

        public ActiveReceiptEmailService(
            IFrontLinkService frontLinkService,
            IEmailTemplateService emailTemplateService,
            ICoreEmailService coreEmailService)
        {
            this.frontLinkService = frontLinkService;
            this.emailTemplateService = emailTemplateService;
            this.coreEmailService = coreEmailService;
        }

        public async Task SendActiveReceiptEmail(string email, string id, string recipientName, string recipientEmail, long sendTime)
        {
            string sendTimeString = TimeUtils.DateTimeToString(sendTime);
            string cancelationLink = frontLinkService.GetCancelationLink(id);

            string htmlContent = await emailTemplateService.GetEmailTemplate("ActiveReceipt");           // TODO: use some kind of constant? (e.g., a config file to map ACTIVE_RECEIPT to actual template name, supervised by ETS)
            htmlContent = htmlContent.Replace("[RecipientName]", recipientName);                         // TODO: use some kind of constant? (e.g., a config file to map RECIPIENT_NAME to "[RecipientName]")
            htmlContent = htmlContent.Replace("[RecipientEmail]", recipientEmail);
            htmlContent = htmlContent.Replace("[SendTime]", sendTimeString);
            htmlContent = htmlContent.Replace("[CancelationLink]", cancelationLink);

            string? subject = HtmlUtils.GetTitle(htmlContent);
            if(subject == null)
            {
                throw new ArgumentNullException("Cannot extract email subject from template");
            }

            await coreEmailService.SendHtml(email, email, subject, htmlContent);
        }
    }
}
