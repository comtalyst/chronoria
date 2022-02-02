using Azure.Storage.Blobs;

namespace Chronoria_WebAPI.Models
{
    public class ActiveBlobServiceClient : BlobServiceClient
    {
        public ActiveBlobServiceClient(string connectionString) : base(connectionString) { }
    }
}
