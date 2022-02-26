using Azure.Storage.Blobs;

namespace Chronoria_WebAPI.Models
{
    public class ArchivedBlobServiceClient : BlobServiceClient
    {
        public ArchivedBlobServiceClient(string connectionString) : base(connectionString) { }
    }
}
