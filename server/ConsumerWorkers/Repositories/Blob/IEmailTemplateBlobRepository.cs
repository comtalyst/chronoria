using Azure.Storage.Blobs;
using Chronoria_ConsumerWorkers.Models;

namespace Chronoria_ConsumerWorkers.Repositories
{
    public interface IEmailTemplateBlobRepository : IGeneralBlobRepository<StaticBlobServiceClient, BlobText> { }
}
