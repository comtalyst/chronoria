namespace Chronoria_ConsumerWorkers.Services
{
    public interface IActiveReceiptEmailService
    {
        public Task SendActiveReceiptEmail(string email, string id, string recipientName, string recipientEmail, long sendTime);
    }
}
