namespace Chronoria_WebAPI.Services
{
    public class SubmissionService : ISubmissionService
    {
        IIdService idService;

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
        )
        {
            // TODO: Check blacklist

            // TODO: Reroute the file to blob storage
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FileName);
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                file.FormFile.CopyTo(stream);
            }

            // TODO: Generate UUID

            // TODO: Put into DB

            // TODO: Confirmation email
        }

        public void SubmitText(
            string senderEmail, 
            string senderName, 
            string recipientEmail, 
            string recipientName, 
            long sendTime, 
            long createTime, 
            string text
        )
        {
            // TODO: Check blacklist

            // TODO: Generate UUID

            // TODO: Put into DB

            // TODO: Confirmation email
        }
    }
}
