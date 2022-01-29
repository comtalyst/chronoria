using Chronoria_WebAPI.Repositories;
using Chronoria_WebAPI.Models;
using Chronoria_WebAPI.utils;

namespace Chronoria_WebAPI.Services
{
    public class SubmissionService : ISubmissionService
    {
        IIdService idService;
        ICapsuleRepository<PendingContext> pendingCapsuleRepo;
        IFileContentRepository<PendingContext> pendingFileContentRepo;
        ITextContentRepository<PendingContext> pendingTextContentRepo;

        public void SubmitFile(
            string senderEmail, 
            string senderName, 
            string recipientEmail, 
            string recipientName, 
            long sendTime, 
            int textLocation, 
            string text, 
            Models.File file
        )
        {
            // TODO: Check blacklist

            // TODO: Reroute the file to blob storage and retrieve a file ID
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FileName);
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                file.FormFile.CopyTo(stream);
            }
            string fileId = "";

            // TODO: Reroute the text to blob storage and retrieve a text file ID
            string textFileId = "";

            // Generate UUID
            string id = idService.generate();

            // Put into DB
            FileContent fileContent = new FileContent(id, fileId, file.FileName, textLocation, textFileId);
            pendingFileContentRepo.Create(fileContent);

            Capsule capsule = new Capsule(
                id, 
                senderEmail, 
                senderName, 
                recipientEmail, 
                recipientName, 
                1, 
                TimeUtils.EpochMsToDateTime(sendTime),
                TimeUtils.now(), 
                0
            );  // TODO: enums
            pendingCapsuleRepo.Create(capsule);

            // TODO: Confirmation email
        }

        public void SubmitText(
            string senderEmail, 
            string senderName, 
            string recipientEmail, 
            string recipientName, 
            long sendTime, 
            string text
        )
        {
            // TODO: Check blacklist

            // TODO: Reroute the text to blob storage and retrieve a text file ID
            string textFileId = "";

            // Generate UUID
            string id = idService.generate();

            // Put into DB
            TextContent textContent = new TextContent(id, textFileId);
            pendingTextContentRepo.Create(textContent);

            Capsule capsule = new Capsule(
                id,
                senderEmail,
                senderName,
                recipientEmail,
                recipientName,
                1,
                TimeUtils.EpochMsToDateTime(sendTime),
                TimeUtils.now(),
                0
            );  // TODO: enums
            pendingCapsuleRepo.Create(capsule);

            // TODO: Confirmation email
        }
    }
}
