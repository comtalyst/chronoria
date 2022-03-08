namespace Chronoria_ConsumerWorkers.Services
{
    public interface IDeliveryReceiptEmailService
    {
        public Task SendDeliveryReceiptEmail(string email, string recipientName, string recipientEmail, long createTime, long sendTime);
    }
}
