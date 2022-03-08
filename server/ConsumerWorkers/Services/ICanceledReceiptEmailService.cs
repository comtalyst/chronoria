namespace Chronoria_ConsumerWorkers.Services
{
    public interface ICanceledReceiptEmailService
    {
        public Task SendCanceledReceiptEmail(string email, string recipientName, string recipientEmail, long sendTime);
    }
}
