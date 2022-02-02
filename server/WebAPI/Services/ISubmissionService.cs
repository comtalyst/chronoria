﻿namespace Chronoria_WebAPI.Services
{
    public interface ISubmissionService
    {
        public void SubmitFile(
            string senderEmail,
            string senderName,
            string recipientEmail,
            string recipientName,
            long sendTime,
            int textLocation,
            string text,
            Models.UploadedFile file
        );

        public void SubmitText(
            string senderEmail,
            string senderName,
            string recipientEmail,
            string recipientName,
            long sendTime,
            string text
        );
    }
}
