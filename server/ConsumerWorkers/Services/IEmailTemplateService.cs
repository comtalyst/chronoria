namespace Chronoria_ConsumerWorkers.Services
{
    public interface IEmailTemplateService
    {
        public Task<string> GetEmailTemplate(string templateName);
    }
}
