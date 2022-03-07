using Azure.Storage.Blobs;

namespace Chronoria_ConsumerWorkers.Models
{
    public class ActiveBlobServiceClient : BlobServiceClient
    {
        public ActiveBlobServiceClient(string connectionString) : base(connectionString) { }
    }
}
