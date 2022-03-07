using Azure.Storage.Blobs;

namespace Chronoria_ConsumerWorkers.Models
{
    public class PendingBlobServiceClient : BlobServiceClient
    {
        public PendingBlobServiceClient(string connectionString) : base(connectionString) { }
    }
}
