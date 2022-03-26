using Chronoria_ConsumerWorkers.Models;
using Chronoria_ConsumerWorkers.utils;

namespace Chronoria_ConsumerWorkers.Services
{
    public class CapsuleDeliveryService : ICapsuleDeliveryService
    {
        private readonly IFrontLinkService frontLinkService;
        private readonly IEmailTemplateService emailTemplateService;
        private readonly ICoreEmailService coreEmailService;

        public CapsuleDeliveryService(
            IFrontLinkService frontLinkService,
            IEmailTemplateService emailTemplateService,
            ICoreEmailService coreEmailService)
        {
            this.frontLinkService = frontLinkService;
            this.emailTemplateService = emailTemplateService;
            this.coreEmailService = coreEmailService;
        }

        public async Task DeliverFile(string id, string senderEmail, string senderName, string recipientEmail, string recipientName, long sendTime, long createTime, string text, TextLocation textLocation, string fileRef)
        {
            // TODO: implement the use of TextLocation
            string sendTimeString = TimeUtils.DateTimeToString(sendTime);
            string createTimeString = TimeUtils.DateTimeToString(createTime);
            string downloadLink = frontLinkService.GetDownloadLink(id);
            string totalTimeString = TimeUtils.TimeSpanToString(createTime, sendTime);

            string emailTemplateName;
            if (senderEmail.Equals(recipientEmail))
                emailTemplateName = "SelfFileCapsuleDelivery";
            else
                emailTemplateName = "P2pFileCapsuleDelivery";

            string htmlContent = await emailTemplateService.GetEmailTemplate(emailTemplateName);

            htmlContent = htmlContent.Replace("[SenderName]", senderName);
            htmlContent = htmlContent.Replace("[SenderEmail]", senderEmail);
            htmlContent = htmlContent.Replace("[RecipientName]", recipientName);
            htmlContent = htmlContent.Replace("[RecipientEmail]", recipientEmail);
            htmlContent = htmlContent.Replace("[SendTime]", sendTimeString);
            htmlContent = htmlContent.Replace("[CreateTime]", createTimeString);
            htmlContent = htmlContent.Replace("[TotalTime]", totalTimeString);
            htmlContent = htmlContent.Replace("[DownloadLink]", downloadLink);

            // content replace must be after .Replace()s to prevent text modification
            string textHtml = "<p>" + text.Replace("\n", "<br/>") + "</p>";
            htmlContent = htmlContent.Replace("[Content]", textHtml);

            string? subject = HtmlUtils.GetTitle(htmlContent);
            if (subject == null)
            {
                throw new ArgumentNullException("Cannot extract email subject from template");
            }

            await coreEmailService.SendHtml(recipientEmail, recipientName, subject, htmlContent);
        }

        public async Task DeliverText(string id, string senderEmail, string senderName, string recipientEmail, string recipientName, long sendTime, long createTime, string text)
        {
            string sendTimeString = TimeUtils.DateTimeToString(sendTime);
            string createTimeString = TimeUtils.DateTimeToString(createTime);
            string totalTimeString = TimeUtils.TimeSpanToString(createTime, sendTime);

            string emailTemplateName;
            if (senderEmail.Equals(recipientEmail))
                emailTemplateName = "SelfTextCapsuleDelivery";
            else
                emailTemplateName = "P2pTextCapsuleDelivery";

            string htmlContent = await emailTemplateService.GetEmailTemplate(emailTemplateName);

            htmlContent = htmlContent.Replace("[SenderName]", senderName);
            htmlContent = htmlContent.Replace("[SenderEmail]", senderEmail);
            htmlContent = htmlContent.Replace("[RecipientName]", recipientName);
            htmlContent = htmlContent.Replace("[RecipientEmail]", recipientEmail);
            htmlContent = htmlContent.Replace("[SendTime]", sendTimeString);
            htmlContent = htmlContent.Replace("[CreateTime]", createTimeString);
            htmlContent = htmlContent.Replace("[TotalTime]", totalTimeString);

            // content replace must be after .Replace()s to prevent text modification
            string textHtml = "<p>" + text.Replace("\n", "<br/>") + "</p>";
            htmlContent = htmlContent.Replace("[Content]", textHtml);

            string? subject = HtmlUtils.GetTitle(htmlContent);
            if (subject == null)
            {
                throw new ArgumentNullException("Cannot extract email subject from template");
            }

            await coreEmailService.SendHtml(recipientEmail, recipientName, subject, htmlContent);
        }
    }
}
