using Azure.Storage.Blobs;

namespace Chronoria_ConsumerWorkers.Models
{
    public class StaticBlobServiceClient : BlobServiceClient            // should change to Azure Files?
    {
        public StaticBlobServiceClient(string connectionString) : base(connectionString) { }
    }
}
