using Chronoria_WebAPI.Models;
using Chronoria_WebAPI.Repositories;
using Chronoria_WebAPI.Producers;
using Chronoria_WebAPI.utils;

namespace Chronoria_WebAPI.Services
{
    public class CancelationService : ICancelationService
    {
        private ICapsuleRepository<ActiveContext> activeCapsuleRepo;
        private IFileContentRepository<ActiveContext> activeFileContentRepo;
        private ITextContentRepository<ActiveContext> activeTextContentRepo;

        private ICapsuleRepository<ArchivedContext> archivedCapsuleRepo;
        private IFileContentRepository<ArchivedContext> archivedFileContentRepo;
        private ITextContentRepository<ArchivedContext> archivedTextContentRepo;

        private IFileBlobRepository<ActiveBlobServiceClient> activeFileBlobRepo;
        private ITextBlobRepository<ActiveBlobServiceClient> activeTextBlobRepo;

        private IFileBlobRepository<ArchivedBlobServiceClient> archivedFileBlobRepo;
        private ITextBlobRepository<ArchivedBlobServiceClient> archivedTextBlobRepo;

        private ICanceledReceiptEmailProducer canceledReceiptEmailProducer;
        public CancelationService(
            ICapsuleRepository<ActiveContext> activeCapsuleRepo,
            IFileContentRepository<ActiveContext> activeFileContentRepo,
            ITextContentRepository<ActiveContext> activeTextContentRepo,
            IFileBlobRepository<ActiveBlobServiceClient> activeFileBlobRepo,
            ITextBlobRepository<ActiveBlobServiceClient> activeTextBlobRepo,
            ICapsuleRepository<ArchivedContext> archivedCapsuleRepo,
            IFileContentRepository<ArchivedContext> archivedFileContentRepo,
            ITextContentRepository<ArchivedContext> archivedTextContentRepo,
            IFileBlobRepository<ArchivedBlobServiceClient> archivedFileBlobRepo,
            ITextBlobRepository<ArchivedBlobServiceClient> archivedTextBlobRepo,
            ICanceledReceiptEmailProducer canceledReceiptEmailProducer)
        {
            this.activeCapsuleRepo = activeCapsuleRepo;
            this.activeFileContentRepo = activeFileContentRepo;
            this.activeTextContentRepo = activeTextContentRepo;
            this.activeTextBlobRepo = activeTextBlobRepo;
            this.activeFileBlobRepo = activeFileBlobRepo;
            this.activeTextBlobRepo = activeTextBlobRepo;

            this.archivedCapsuleRepo = archivedCapsuleRepo;
            this.archivedFileContentRepo = archivedFileContentRepo;
            this.archivedTextContentRepo = archivedTextContentRepo;
            this.archivedTextBlobRepo = archivedTextBlobRepo;
            this.archivedFileBlobRepo = archivedFileBlobRepo;
            this.archivedTextBlobRepo = archivedTextBlobRepo;

            this.canceledReceiptEmailProducer = canceledReceiptEmailProducer;
        }
        public async Task Cancel(string id)
        {
            // Get from active DB
            var capsule = await activeCapsuleRepo.Get(id);
            if (capsule == null)
                throw new RejectException(RejectException.CapsuleNotFoundOrReleased);

            if (capsule.ContentType == ContentType.Text)
            {
                var content = await activeTextContentRepo.Get(id);
                if (content == null)
                    throw new RejectException(RejectException.ContentNotFoundOrReleased);

                // Remove from active DB
                await activeCapsuleRepo.Delete(id);
                await activeTextContentRepo.Delete(id);

                // Transfer blob files
                var uri = activeTextBlobRepo.GetTransferUri(content.TextFileId);
                if (uri == null)
                    throw new RejectException(RejectException.TextBlobNotFoundOrReleased);

                try
                {
                    await archivedTextBlobRepo.ReceiveTransfer(content.TextFileId, uri);
                }
                catch (Exception)
                {
                    throw new RejectException(RejectException.TransferFailedOrReleased);
                }
                await activeTextBlobRepo.Delete(content.TextFileId);

                // Add to archived DB
                var newCapsule = new Capsule(capsule);
                newCapsule.Status = Status.Canceled;
                await archivedTextContentRepo.Create(content);
                await archivedCapsuleRepo.Create(newCapsule);

                // Send receipt email
                await canceledReceiptEmailProducer.Produce(new CanceledReceiptEmailMessage(newCapsule.SenderEmail, id,
                    newCapsule.RecipientName,
                    newCapsule.RecipientEmail,
                    TimeUtils.DateTimeToEpochMs(newCapsule.SendTime)
                    ));
            }
            else if (capsule.ContentType == ContentType.File)
            {
                var content = await activeFileContentRepo.Get(id);
                if (content == null)
                    throw new RejectException(RejectException.ContentNotFoundOrReleased);

                // Remove from active DB
                await activeCapsuleRepo.Delete(id);
                await activeFileContentRepo.Delete(id);

                // Transfer blob files
                var uriText = activeTextBlobRepo.GetTransferUri(content.TextFileId);
                if (uriText == null)
                    throw new RejectException(RejectException.TextBlobNotFoundOrReleased);
                var uriFile = activeFileBlobRepo.GetTransferUri(content.FileId);
                if (uriFile == null)
                    throw new RejectException(RejectException.FileBlobNotFoundOrReleased);

                // we could run them concorrently
                var textTransferTask = archivedTextBlobRepo.ReceiveTransfer(content.TextFileId, uriText);
                var fileTransferTask = archivedFileBlobRepo.ReceiveTransfer(content.FileId, uriFile);
                try
                {
                    await textTransferTask;
                    await fileTransferTask;
                }
                catch (Exception)
                {
                    throw new RejectException(RejectException.TransferFailedOrReleased);
                }
                await activeTextBlobRepo.Delete(content.TextFileId);
                await activeFileBlobRepo.Delete(content.FileId);

                // Add to archived DB
                var newCapsule = new Capsule(capsule);
                newCapsule.Status = Status.Canceled;
                await archivedFileContentRepo.Create(content);
                await archivedCapsuleRepo.Create(newCapsule);

                // Send receipt email
                await canceledReceiptEmailProducer.Produce(new CanceledReceiptEmailMessage(newCapsule.SenderEmail, id,
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
