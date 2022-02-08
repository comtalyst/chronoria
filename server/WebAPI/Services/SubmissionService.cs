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

        IFileBlobRepository<PendingBlobServiceClient> pendingFileBlobRepo;

        public async Task SubmitFile(
            string senderEmail, 
            string senderName, 
            string recipientEmail, 
            string recipientName, 
            long sendTime, 
            int textLocation, 
            string text, 
            UploadedFile file
        )
        {
            // TODO: Check blacklist

            // Generate UUID
            string id = idService.generate();

            // Reroute the file to blob storage and retrieve a file ID
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FileName);
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                file.FormFile.CopyTo(stream);
            }
            string fileId = id + "-0";      // new file name for blob storage

            BlobFile blobFile = new BlobFile(fileId, file.FormFile);
            await pendingFileBlobRepo.Create(blobFile);


            // TODO: Reroute the text to blob storage and retrieve a text file ID
            string textFileId = id + "-body-0";

            // Put into DB
            FileContent fileContent = new FileContent(id, fileId, file.FileName, textLocation, textFileId);
            await pendingFileContentRepo.Create(fileContent);

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
            await pendingCapsuleRepo.Create(capsule);

            // TODO: Confirmation email
        }

        public async Task SubmitText(
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
