using Azure.Storage.Blobs;
using Chronoria_ConsumerWorkers.Models;

namespace Chronoria_ConsumerWorkers.Repositories
{
    public class FileBlobRepository<BlobServiceClientType> : BlobRepositoryHelpers<BlobServiceClientType>, IFileBlobRepository<BlobServiceClientType> where BlobServiceClientType : BlobServiceClient
    {
        public FileBlobRepository(BlobServiceClientType client, string containerName) : base(client, containerName) { }

        public async Task<BlobFile> Create(BlobFile entry)
        {
            BlobClient blobClient = GetClient(entry.BlobFileName);
            using(var stream = entry.FormFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream);
            }
            return entry;
        }

        public async Task<BlobFile> Get(string blobFileName)
        {
            throw new NotImplementedException();
        }
    }
}
