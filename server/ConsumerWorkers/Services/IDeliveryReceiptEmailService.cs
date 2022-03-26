namespace Chronoria_ConsumerWorkers.Services
{
    public interface IDeliveryReceiptEmailService
    {
        // not needed for now?
        public Task SendDeliveryReceiptEmail(string email, string recipientName, string recipientEmail, long createTime, long sendTime);
    }
}
