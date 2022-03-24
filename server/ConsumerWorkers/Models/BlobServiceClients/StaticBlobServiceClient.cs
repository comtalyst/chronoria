using Azure.Storage.Blobs;

namespace Chronoria_ConsumerWorkers.Models
{
    public class StaticBlobServiceClient : BlobServiceClient
    {
        public StaticBlobServiceClient(string connectionString) : base(connectionString) { }
    }
}
