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
            //var neglectibleTasks = new List<Task>();                          // Concurrency not supported for EF Core

            // Get from pending DB
            var capsule = await pendingCapsuleRepo.Get(id);
            if(capsule == null)
                throw new ArgumentException("CapsuleNotFound;Expired");         // TODO: some protocol with the frontend
            
            if (capsule.ContentType == ContentType.Text)
            {
                var content = await pendingTextContentRepo.Get(id);
                if (content == null)
                    throw new ArgumentException("ContentNotFound;JustExpired");

                // Remove from pending DB                                            // neglectible
                await pendingCapsuleRepo.Delete(id);
                await pendingTextContentRepo.Delete(id);

                // Transfer blob files                                               // time consuming; might move all after this to consumer
                var uri = pendingTextBlobRepo.GetTransferUri(content.TextFileId);
                if (uri == null)
                    throw new ArgumentException("TextNotFound;JustExpired");

                try
                {
                    await activeTextBlobRepo.ReceiveTransfer(content.TextFileId, uri);   // like Gets, must done to guarantee no expire/failure
                }
                catch (Exception)                                                    // could fail if it expires right when transferring
                {
                    throw new ArgumentException("JustExpired");
                }
                await pendingTextBlobRepo.Delete(content.TextFileId);

                // Add to active DB                                                  // should not expect any failure/expire now
                var newCapsule = new Capsule(capsule);
                newCapsule.Status = Status.Active;
                await activeTextContentRepo.Create(content);       // must be ready for send schedule
                await activeCapsuleRepo.Create(newCapsule);            // will trigger the send schedule; most things must be ready before this

                // TODO: send receipt email
            }
            else if (capsule.ContentType == ContentType.File)
            {
                var content = await pendingFileContentRepo.Get(id);
                if (content == null)
                    throw new ArgumentException("ContentNotFound;JustExpired");

                // Remove from pending DB
                await pendingCapsuleRepo.Delete(id);
                await pendingFileContentRepo.Delete(id);

                // Transfer blob files
                var uriText = pendingTextBlobRepo.GetTransferUri(content.TextFileId);
                if (uriText == null)
                    throw new ArgumentException("TextNotFound;JustExpired");
                var uriFile = pendingFileBlobRepo.GetTransferUri(content.FileId);
                if (uriFile == null)
                    throw new ArgumentException("FileNotFound;JustExpired");

                // we could run them concorrently
                var textTransferTask = activeTextBlobRepo.ReceiveTransfer(content.TextFileId, uriText);
                var fileTransferTask = activeFileBlobRepo.ReceiveTransfer(content.FileId, uriFile);
                try
                {
                    await textTransferTask;
                    await fileTransferTask;
                }
                catch (Exception)
                {
                    throw new ArgumentException("JustExpired");
                }
                await pendingTextBlobRepo.Delete(content.TextFileId);
                await pendingFileBlobRepo.Delete(content.FileId);

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
            /*
            while (neglectibleTasks.Any())
            {
                await neglectibleTasks[neglectibleTasks.Count - 1];
                neglectibleTasks.RemoveAt(neglectibleTasks.Count - 1);
            }*/
        }
    }
}
