namespace Chronoria_ConsumerWorkers.Services
{
    public interface IConfEmailService
    {
        public Task SendConfEmail(string email, string id);
    }
}
