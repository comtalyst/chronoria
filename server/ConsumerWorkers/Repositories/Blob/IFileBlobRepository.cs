using Azure.Storage.Blobs;
using Chronoria_ConsumerWorkers.Models;

namespace Chronoria_ConsumerWorkers.Repositories
{
    public interface IFileBlobRepository<BlobServiceClientType> : IGeneralBlobRepository<BlobServiceClientType, BlobFile> where BlobServiceClientType : BlobServiceClient { }
}
