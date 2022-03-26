using Chronoria_ConsumerWorkers.utils;

namespace Chronoria_ConsumerWorkers.Services
{
    public class ConfEmailService : IConfEmailService
    {
        private readonly IFrontLinkService frontLinkService;
        private readonly IEmailTemplateService emailTemplateService;
        private readonly ICoreEmailService coreEmailService;

        public ConfEmailService(
            IFrontLinkService frontLinkService,
            IEmailTemplateService emailTemplateService,
            ICoreEmailService coreEmailService)
        {
            this.frontLinkService = frontLinkService;
            this.emailTemplateService = emailTemplateService;
            this.coreEmailService = coreEmailService;
        }

        public async Task SendConfEmail(string email, string id)
        {
            string confirmationLink = frontLinkService.GetConfirmationLink(id);            // confRef is id

            string htmlContent = await emailTemplateService.GetEmailTemplate("Confirmation");
            htmlContent = htmlContent.Replace("[ConfirmationLink]", confirmationLink);

            string? subject = HtmlUtils.GetTitle(htmlContent);
            if (subject == null)
            {
                throw new ArgumentNullException("Cannot extract email subject from template");
            }

            await coreEmailService.SendHtml(email, email, subject, htmlContent);
        }
    }
}
