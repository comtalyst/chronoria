using Chronoria_WebAPI.Repositories;
using Chronoria_WebAPI.Models;
using Chronoria_WebAPI.Producers;
using Chronoria_WebAPI.utils;

namespace Chronoria_WebAPI.Services
{
    public class SubmissionService : ISubmissionService
    {
        private IIdService idService;

        private ICapsuleRepository<PendingContext> pendingCapsuleRepo;
        private IFileContentRepository<PendingContext> pendingFileContentRepo;
        private ITextContentRepository<PendingContext> pendingTextContentRepo;

        private IFileBlobRepository<PendingBlobServiceClient> pendingFileBlobRepo;
        private ITextBlobRepository<PendingBlobServiceClient> pendingTextBlobRepo;

        private IConfEmailProducer confEmailProducer;

        public SubmissionService(
            IIdService idService,
            ICapsuleRepository<PendingContext> pendingCapsuleRepo,
            IFileContentRepository<PendingContext> pendingFileContentRepo,
            ITextContentRepository<PendingContext> pendingTextContentRepo,
            IFileBlobRepository<PendingBlobServiceClient> pendingFileBlobRepo,
            ITextBlobRepository<PendingBlobServiceClient> pendingTextBlobRepo,
            IConfEmailProducer confEmailProducer)
        {
            this.idService = idService;
            this.pendingCapsuleRepo = pendingCapsuleRepo;
            this.pendingFileContentRepo = pendingFileContentRepo;
            this.pendingTextContentRepo = pendingTextContentRepo;
            this.pendingTextBlobRepo = pendingTextBlobRepo;
            this.pendingFileBlobRepo = pendingFileBlobRepo;
            this.pendingTextBlobRepo = pendingTextBlobRepo;
            this.confEmailProducer = confEmailProducer;
        }

        public async Task SubmitFile(
            string senderEmail, 
            string senderName, 
            string recipientEmail, 
            string recipientName, 
            long sendTime,
            string textLocation, 
            string text, 
            UploadedFile file
        )
        {
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


            // Reroute the text to blob storage and retrieve a text file ID
            string textFileId = id + "-body-0";
            BlobText blobText = new BlobText(textFileId, text);
            await pendingTextBlobRepo.Create(blobText);

            // Put into DB
            FileContent fileContent = new FileContent(id, fileId, file.FileName, (TextLocation)Enum.Parse(typeof(TextLocation), textLocation), textFileId);
            await pendingFileContentRepo.Create(fileContent);

            Capsule capsule = new Capsule(
                id, 
                senderEmail, 
                senderName, 
                recipientEmail, 
                recipientName, 
                ContentType.File, 
                TimeUtils.EpochMsToDateTime(sendTime),
                TimeUtils.now(),
                Status.Pending
            );
            await pendingCapsuleRepo.Create(capsule);

            // Confirmation email
            await confEmailProducer.Produce(new ConfEmailMessage(senderEmail, id));
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
            // Generate UUID
            string id = idService.generate();

            // Reroute the text to blob storage and retrieve a text file ID
            string textFileId = id + "-body-0";
            BlobText blobText = new BlobText(textFileId, text);
            await pendingTextBlobRepo.Create(blobText);

            // Put into DB
            TextContent textContent = new TextContent(id, textFileId);          // not the actual content, just the table name that stores blob text file name
            await pendingTextContentRepo.Create(textContent);

            Capsule capsule = new Capsule(
                id,
                senderEmail,
                senderName,
                recipientEmail,
                recipientName,
                ContentType.Text,
                TimeUtils.EpochMsToDateTime(sendTime),
                TimeUtils.now(),
                Status.Pending
            );
            await pendingCapsuleRepo.Create(capsule);

            // Confirmation email
            await confEmailProducer.Produce(new ConfEmailMessage(senderEmail, id));
        }
    }
}
