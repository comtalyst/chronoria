using Chronoria_WebAPI.Models;
using Chronoria_WebAPI.Repositories;
using Chronoria_WebAPI.Producers;
using Chronoria_WebAPI.utils;

namespace Chronoria_WebAPI.Services
{
    public class ConfirmationService : IConfirmationService
    {
        private ICapsuleRepository<PendingContext> pendingCapsuleRepo;
        private IFileContentRepository<PendingContext> pendingFileContentRepo;
        private ITextContentRepository<PendingContext> pendingTextContentRepo;

        private ICapsuleRepository<ActiveContext> activeCapsuleRepo;
        private IFileContentRepository<ActiveContext> activeFileContentRepo;
        private ITextContentRepository<ActiveContext> activeTextContentRepo;

        private IFileBlobRepository<PendingBlobServiceClient> pendingFileBlobRepo;
        private ITextBlobRepository<PendingBlobServiceClient> pendingTextBlobRepo;

        private IFileBlobRepository<ActiveBlobServiceClient> activeFileBlobRepo;
        private ITextBlobRepository<ActiveBlobServiceClient> activeTextBlobRepo;

        private IActiveReceiptEmailProducer activeReceiptEmailProducer;
        public ConfirmationService(
            ICapsuleRepository<PendingContext> pendingCapsuleRepo,
            IFileContentRepository<PendingContext> pendingFileContentRepo,
            ITextContentRepository<PendingContext> pendingTextContentRepo,
            IFileBlobRepository<PendingBlobServiceClient> pendingFileBlobRepo,
            ITextBlobRepository<PendingBlobServiceClient> pendingTextBlobRepo,
            ICapsuleRepository<ActiveContext> activeCapsuleRepo,
            IFileContentRepository<ActiveContext> activeFileContentRepo,
            ITextContentRepository<ActiveContext> activeTextContentRepo,
            IFileBlobRepository<ActiveBlobServiceClient> activeFileBlobRepo,
            ITextBlobRepository<ActiveBlobServiceClient> activeTextBlobRepo,
            IActiveReceiptEmailProducer activeReceiptEmailProducer)
        {
            this.pendingCapsuleRepo = pendingCapsuleRepo;
            this.pendingFileContentRepo = pendingFileContentRepo;
            this.pendingTextContentRepo = pendingTextContentRepo;
            this.pendingTextBlobRepo = pendingTextBlobRepo;
            this.pendingFileBlobRepo = pendingFileBlobRepo;
            this.pendingTextBlobRepo = pendingTextBlobRepo;

            this.activeCapsuleRepo = activeCapsuleRepo;
            this.activeFileContentRepo = activeFileContentRepo;
            this.activeTextContentRepo = activeTextContentRepo;
            this.activeTextBlobRepo = activeTextBlobRepo;
            this.activeFileBlobRepo = activeFileBlobRepo;
            this.activeTextBlobRepo = activeTextBlobRepo;

            this.activeReceiptEmailProducer = activeReceiptEmailProducer;
        }
        public async Task Confirm(string id)
        {
            // Acquire the capsule
            var capsule = await pendingCapsuleRepo.Retrieve(id);
            if(capsule == null)
                throw new RejectException(RejectException.CapsuleNotFoundOrExpired);
            
            if (capsule.ContentType == ContentType.Text)
            {
                // Get and Delete TextContent
                var content = await pendingTextContentRepo.Retrieve(id);
                if (content == null)
                    throw new NullReferenceException("TextContent is missing");

                // Transfer blob files                                               // time consuming; might move all after this to consumer
                var uri = pendingTextBlobRepo.GetTransferUri(content.TextFileId);
                if (uri == null)
                    throw new NullReferenceException("TextBlob is missing");
                await activeTextBlobRepo.ReceiveTransfer(content.TextFileId, uri);   // like Gets, must done to guarantee no failure

                // Delete blob file
                try
                {
                    await pendingTextBlobRepo.Delete(content.TextFileId);
                }
                catch (Exception)
                {
                    // Not a big deal, we can continue
                    // TODO: log
                }

                // Add to active DB
                var newCapsule = new Capsule(capsule);
                newCapsule.Status = Status.Active;
                // Warning: if these fail (e.g., DB connection just goes down) then data will be lost as they were deleted before
                await activeTextContentRepo.Create(content);            // must be ready for send schedule
                await activeCapsuleRepo.Create(newCapsule);            // will trigger the send schedule; most things must be ready before this

                // Send receipt email
                await activeReceiptEmailProducer.Produce(new ActiveReceiptEmailMessage(newCapsule.SenderEmail, id,
                    newCapsule.RecipientName,
                    newCapsule.RecipientEmail,
                    TimeUtils.DateTimeToEpochMs(newCapsule.SendTime)
                    ));
            }
            else if (capsule.ContentType == ContentType.File)
            {
                // Get and Delete TextContent
                var content = await pendingFileContentRepo.Retrieve(id);
                if (content == null)
                    throw new NullReferenceException("FileContent is missing");

                // Transfer blob files
                var uriText = pendingTextBlobRepo.GetTransferUri(content.TextFileId);
                if (uriText == null)
                    throw new NullReferenceException("TextBlob is missing");
                var uriFile = pendingFileBlobRepo.GetTransferUri(content.FileId);
                if (uriFile == null)
                    throw new NullReferenceException("FileBlob is missing");

                // we could run them concorrently
                var textTransferTask = activeTextBlobRepo.ReceiveTransfer(content.TextFileId, uriText);
                var fileTransferTask = activeFileBlobRepo.ReceiveTransfer(content.FileId, uriFile);
                await textTransferTask;
                await fileTransferTask;
                // Delete blob file
                try
                {
                    await pendingTextBlobRepo.Delete(content.TextFileId);
                }
                catch (Exception)
                {
                    // Not a big deal, we can continue
                    // TODO: log
                }
                try
                {
                    await pendingFileBlobRepo.Delete(content.FileId);
                }
                catch (Exception)
                {
                    // TODO: log
                }

                // Add to active DB
                var newCapsule = new Capsule(capsule);
                newCapsule.Status = Status.Active;
                await activeFileContentRepo.Create(content);
                await activeCapsuleRepo.Create(newCapsule);

                // Send receipt email
                await activeReceiptEmailProducer.Produce(new ActiveReceiptEmailMessage(newCapsule.SenderEmail, id, 
                    newCapsule.RecipientName, 
                    newCapsule.RecipientEmail, 
                    TimeUtils.DateTimeToEpochMs(newCapsule.SendTime)
                    ));
            }
            else
            {
                throw new NotImplementedException("Unknown Capsule Type");
            }
        }
    }
}
