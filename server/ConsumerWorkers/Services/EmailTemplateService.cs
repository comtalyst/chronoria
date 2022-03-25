using Chronoria_ConsumerWorkers.Repositories;

namespace Chronoria_ConsumerWorkers.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IEmailTemplateBlobRepository emailTemplateBlobRepository;
        public EmailTemplateService(IEmailTemplateBlobRepository emailTemplateBlobRepository)
        {
            this.emailTemplateBlobRepository = emailTemplateBlobRepository;
        }

        public async Task<string> GetEmailTemplate(string templateName)
        {
            string templateFileName = templateName + "Email.html";
            return (await emailTemplateBlobRepository.Get(templateFileName)).content;
        }
    }
}
