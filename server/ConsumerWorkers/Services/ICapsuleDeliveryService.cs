namespace Chronoria_ConsumerWorkers.Services
{
    public enum TextLocation
    {
        Before,
        After
    }
    public interface ICapsuleDeliveryService
    {
        public Task DeliverText(
            string id,
            string senderEmail,
            string senderName,
            string receipientEmail,
            string receipientName,
            string sendTime,
            string createTime,
            string text
            );
        public Task DeliverFile(
            string id,
            string senderEmail,
            string senderName,
            string receipientEmail,
            string receipientName,
            string sendTime,
            string createTime,
            string text,
            TextLocation textLocation,
            string fileRef
            );
    }
}
