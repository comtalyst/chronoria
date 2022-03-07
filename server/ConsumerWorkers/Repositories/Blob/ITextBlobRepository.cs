using Azure.Storage.Blobs;
using Chronoria_ConsumerWorkers.Models;

namespace Chronoria_ConsumerWorkers.Repositories
{
    public interface ITextBlobRepository<BlobServiceClientType> : IGeneralBlobRepository<BlobServiceClientType, BlobText> where BlobServiceClientType : BlobServiceClient { }
}
