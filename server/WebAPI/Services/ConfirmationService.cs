using Chronoria_WebAPI.Models;
using Chronoria_WebAPI.Repositories;

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
            ITextBlobRepository<ActiveBlobServiceClient> activeTextBlobRepo)
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
        }
        public async Task Confirm(string id)
        {
            var neglectibleTasks = new List<Task>();

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
                neglectibleTasks.Add(pendingCapsuleRepo.Delete(id));
                neglectibleTasks.Add(pendingTextContentRepo.Delete(id));

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

                // Add to active DB                                                  // should not expect any failure/expire now
                await activeTextContentRepo.Create(content);       // must be ready for send schedule
                await activeCapsuleRepo.Create(capsule);            // will trigger the send schedule; most things must be ready before this

                // TODO: send receipt email
            }
            else if (capsule.ContentType == ContentType.File)
            {
                var content = await pendingFileContentRepo.Get(id);
                if (content == null)
                    throw new ArgumentException("ContentNotFound;JustExpired");

                // Remove from pending DB
                neglectibleTasks.Add(pendingCapsuleRepo.Delete(id));
                neglectibleTasks.Add(pendingFileContentRepo.Delete(id));

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

                // Add to active DB
                await activeFileContentRepo.Create(content);
                await activeCapsuleRepo.Create(capsule);

                // TODO: send receipt email
            }
            else
            {
                throw new NotImplementedException("Unknown Capsule Type");
            }
        }
    }
}
