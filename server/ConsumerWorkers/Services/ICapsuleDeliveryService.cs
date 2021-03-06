using Chronoria_ConsumerWorkers.Models;

namespace Chronoria_ConsumerWorkers.Services
{
    public interface ICapsuleDeliveryService
    {
        public Task DeliverText(
            string id,
            string senderEmail,
            string senderName,
            string recipientEmail,
            string recipientName,
            long sendTime,
            long createTime,
            string text
            );
        public Task DeliverFile(
            string id,
            string senderEmail,
            string senderName,
            string recipientEmail,
            string recipientName,
            long sendTime,
            long createTime,
            string text,
            TextLocation textLocation,
            string fileRef
            );
    }
}
