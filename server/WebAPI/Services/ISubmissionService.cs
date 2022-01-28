namespace Chronoria_WebAPI.Services
{
    public interface ISubmissionService
    {
        public void SubmitFile(
            string senderEmail,
            string senderName,
            string recipientEmail,
            string recipientName,
            long sendTime,
            long createTime,
            int textLocation,
            string text,
            Models.File file
        );

        public void SubmitText(
            string senderEmail,
            string senderName,
            string recipientEmail,
            string recipientName,
            long sendTime,
            long createTime,
            string text
        );
    }
}
