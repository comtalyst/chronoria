using Chronoria_ConsumerWorkers.utils;
using Chronoria_ConsumerWorkers.Repositories;
using Chronoria_ConsumerWorkers.Models;
using Chronoria_ConsumerWorkers.Producers;

namespace Chronoria_ConsumerWorkers.Services
{
    public class CapsuleReleaseService : ICapsuleReleaseService
    {
        private readonly ICapsuleRepository<ActiveContext> activeCapsuleRepository;
        private readonly ITextContentRepository<ActiveContext> activeTextContentRepository;
        private readonly IFileContentRepository<ActiveContext> activeFileContentRepository;
        private readonly IFileBlobRepository<ActiveBlobServiceClient> activeFileBlobRepo;
        private readonly ITextBlobRepository<ActiveBlobServiceClient> activeTextBlobRepo;

        private readonly ICapsuleRepository<ArchivedContext> archivedCapsuleRepository;
        private readonly ITextContentRepository<ArchivedContext> archivedTextContentRepository;
        private readonly IFileContentRepository<ArchivedContext> archivedFileContentRepository;
        private readonly IFileBlobRepository<ArchivedBlobServiceClient> archivedFileBlobRepo;
        private readonly ITextBlobRepository<ArchivedBlobServiceClient> archivedTextBlobRepo;

        private readonly ICapsuleDeliveryProducer capsuleDeliveryProducer;

        public CapsuleReleaseService(
            ICapsuleRepository<ActiveContext> activeCapsuleRepository,
            ITextContentRepository<ActiveContext> activeTextContentRepository,
            IFileContentRepository<ActiveContext> activeFileContentRepository,
            IFileBlobRepository<ActiveBlobServiceClient> activeFileBlobRepository,
            ITextBlobRepository<ActiveBlobServiceClient> activeTextBlobRepository,

            ICapsuleRepository<ArchivedContext> archivedCapsuleRepository,
            ITextContentRepository<ArchivedContext> archivedTextContentRepository,
            IFileContentRepository<ArchivedContext> archivedFileContentRepository,
            IFileBlobRepository<ArchivedBlobServiceClient> archivedFileBlobRepository,
            ITextBlobRepository<ArchivedBlobServiceClient> archivedTextBlobRepository,

            ICapsuleDeliveryProducer capsuleDeliveryProducer
            )
        {
            this.activeCapsuleRepository = activeCapsuleRepository;
            this.activeFileContentRepository = activeFileContentRepository;
            this.activeTextContentRepository = activeTextContentRepository;
            this.activeFileBlobRepo = activeFileBlobRepository;
            this.activeTextBlobRepo = activeTextBlobRepository;
            this.archivedCapsuleRepository = archivedCapsuleRepository;
            this.archivedFileContentRepository = archivedFileContentRepository;
            this.archivedTextContentRepository = archivedTextContentRepository;
            this.archivedFileBlobRepo = archivedFileBlobRepository;
            this.archivedTextBlobRepo = archivedTextBlobRepository;

            this.capsuleDeliveryProducer = capsuleDeliveryProducer;
        }

        public async Task ReleaseRange(long timeL, long timeR)
        {
            await ReleaseRange(TimeUtils.EpochMsToDateTime(timeL), TimeUtils.EpochMsToDateTime(timeR));
        }

        public async Task ReleaseRange(DateTime timeL, DateTime timeR)
        {
            // Get all caps
            var preCaps = await activeCapsuleRepository.GetBySendTimeRange(timeL, timeR);
            // Since each capsule always due after confirmation/expiration date (e.g., 24hr),
            // it is safe to assume that all newly added capsules will be added in the future timestamp.
            // And timeR is always less than or equal to current time, no new capsules will be added during this gap.

            // Retrieve each capsule
            foreach(var preCap in preCaps)
            {
                var capsule = await activeCapsuleRepository.Retrieve(preCap.Id);
                if(capsule == null)
                    continue;

                if(capsule.ContentType == ContentType.File)
                {
                    var content = await activeFileContentRepository.Retrieve(capsule.Id);
                    if (content == null)
                        throw new NullReferenceException("FileContent is missing");

                    // Transfer blob files
                    var uriText = activeTextBlobRepo.GetTransferUri(content.TextFileId);
                    if (uriText == null)
                        throw new NullReferenceException("TextBlob is missing");
                    var uriFile = activeFileBlobRepo.GetTransferUri(content.FileId);
                    if (uriFile == null)
                        throw new NullReferenceException("FileBlob is missing");

                    // we could run them concorrently
                    var textTransferTask = archivedTextBlobRepo.ReceiveTransfer(content.TextFileId, uriText);
                    var fileTransferTask = archivedFileBlobRepo.ReceiveTransfer(content.FileId, uriFile);
                    await textTransferTask;
                    await fileTransferTask;
                    // Delete blob file
                    try
                    {
                        await activeTextBlobRepo.Delete(content.TextFileId);
                    }
                    catch (Exception ex)
                    {
                        // Not a big deal, we can continue
                        Console.Error.WriteLine(ex);
                    }
                    try
                    {
                        await activeFileBlobRepo.Delete(content.FileId);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex);
                    }

                    // Add to archived DB
                    var newCapsule = new Capsule(capsule);
                    newCapsule.Status = Status.Released;
                    await archivedFileContentRepository.Create(content);
                    await archivedCapsuleRepository.Create(newCapsule);

                    // Send CapsuleDeliveryMessage
                    var message = new CapsuleDeliveryMessage(
                        newCapsule.Id,
                        newCapsule.SenderEmail,
                        newCapsule.SenderName,
                        newCapsule.RecipientEmail,
                        newCapsule.RecipientName,
                        TimeUtils.DateTimeToEpochMs(newCapsule.SendTime),
                        TimeUtils.DateTimeToEpochMs(newCapsule.CreateTime),
                        (await archivedTextBlobRepo.Get(content.TextFileId)).content,
                        content.TextLocation,
                        content.FileId
                        );
                    await capsuleDeliveryProducer.Produce(message);
                }
                else if (capsule.ContentType == ContentType.Text)
                {
                    var content = await activeTextContentRepository.Retrieve(capsule.Id);
                    if (content == null)
                        throw new NullReferenceException("TextContent is missing");

                    // Transfer blob files
                    var uriText = activeTextBlobRepo.GetTransferUri(content.TextFileId);
                    if (uriText == null)
                        throw new NullReferenceException("TextBlob is missing");

                    await archivedTextBlobRepo.ReceiveTransfer(content.TextFileId, uriText);
                    // Delete blob file
                    try
                    {
                        await activeTextBlobRepo.Delete(content.TextFileId);
                    }
                    catch (Exception ex)
                    {
                        // Not a big deal, we can continue
                        Console.Error.WriteLine(ex);
                    }

                    // Add to archived DB
                    var newCapsule = new Capsule(capsule);
                    newCapsule.Status = Status.Released;
                    await archivedTextContentRepository.Create(content);
                    await archivedCapsuleRepository.Create(newCapsule);

                    // Send CapsuleDeliveryMessage
                    var message = new CapsuleDeliveryMessage(
                        newCapsule.Id,
                        newCapsule.SenderEmail,
                        newCapsule.SenderName,
                        newCapsule.RecipientEmail,
                        newCapsule.RecipientName,
                        TimeUtils.DateTimeToEpochMs(newCapsule.SendTime),
                        TimeUtils.DateTimeToEpochMs(newCapsule.CreateTime),
                        (await archivedTextBlobRepo.Get(content.TextFileId)).content
                        );
                    await capsuleDeliveryProducer.Produce(message);
                }
                else
                {
                    throw new NotImplementedException("Unknown capsule type");
                }
            }
            
        }
    }
}
