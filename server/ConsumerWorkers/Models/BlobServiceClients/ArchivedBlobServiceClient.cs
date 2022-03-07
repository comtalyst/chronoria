using Azure.Storage.Blobs;

namespace Chronoria_ConsumerWorkers.Models
{
    public class ArchivedBlobServiceClient : BlobServiceClient
    {
        public ArchivedBlobServiceClient(string connectionString) : base(connectionString) { }
    }
}
