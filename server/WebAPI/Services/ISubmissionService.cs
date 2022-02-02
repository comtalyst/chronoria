namespace Chronoria_WebAPI.Services
{
    public interface ISubmissionService
    {
        public Task SubmitFile(
            string senderEmail,
            string senderName,
            string recipientEmail,
            string recipientName,
            long sendTime,
            int textLocation,
            string text,
            Models.UploadedFile file
        );

        public Task SubmitText(
            string senderEmail,
            string senderName,
            string recipientEmail,
            string recipientName,
            long sendTime,
            string text
        );
    }
}
