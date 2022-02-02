using Azure.Storage.Blobs;

namespace Chronoria_WebAPI.Models
{
    public class PendingBlobServiceClient : BlobServiceClient
    {
        public PendingBlobServiceClient(string connectionString) : base(connectionString) { }
    }
}
