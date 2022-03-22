using Chronoria_WebAPI.Models;
using Chronoria_WebAPI.Repositories;
using Chronoria_WebAPI.Producers;
using Chronoria_WebAPI.utils;

namespace Chronoria_WebAPI.Services
{
    public class DownloadLinkService : IDownloadLinkService
    {
        private ICapsuleRepository<ArchivedContext> archivedCapsuleRepo;
        private IFileContentRepository<ArchivedContext> archivedFileContentRepo;
        private IFileBlobRepository<ArchivedBlobServiceClient> archivedFileBlobRepo;
        public DownloadLinkService(
            ICapsuleRepository<ArchivedContext> archivedCapsuleRepo,
            IFileContentRepository<ArchivedContext> archivedFileContentRepo,
            IFileBlobRepository<ArchivedBlobServiceClient> archivedFileBlobRepo)
        {
            this.archivedCapsuleRepo = archivedCapsuleRepo;
            this.archivedFileContentRepo = archivedFileContentRepo;
            this.archivedFileBlobRepo = archivedFileBlobRepo;
        }
        public async Task<string> GetLink(string id)
        {
            var capsule = await archivedCapsuleRepo.Get(id);
            if(capsule == null)
                throw new RejectException(RejectException.CapsuleNotFoundOrExpired);
            if (capsule.ContentType == ContentType.Text)
            {
                throw new RejectException(RejectException.NoFileAvailableForThisCapsule);
            }
            else if (capsule.ContentType == ContentType.File)
            {
                // Get and Delete TextContent
                var content = await archivedFileContentRepo.Get(id);
                if (content == null)
                    throw new NullReferenceException("FileContent is missing");

                return archivedFileBlobRepo.GetDownloadLink(content.FileId, content.FileName);
            }
            else
            {
                throw new NotImplementedException("Unknown Capsule Type");
            }
        }
    }
}
