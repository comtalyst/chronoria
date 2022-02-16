using Azure.Storage.Blobs;
using Chronoria_WebAPI.Models;
using System.Text;

namespace Chronoria_WebAPI.Repositories
{
    public class TextBlobRepository<BlobServiceClientType> : BlobRepositoryHelpers<BlobServiceClientType>, ITextBlobRepository<BlobServiceClientType> where BlobServiceClientType : BlobServiceClient
    {
        public TextBlobRepository(BlobServiceClientType client, string containerName) : base(client, containerName) { }

        public async Task<BlobText> Create(BlobText entry)
        {
            BlobClient blobClient = GetClient(entry.BlobFileName);
            await blobClient.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes(entry.content)));
            return entry;
        }

        public async Task Delete(string blobFileName)
        {
            throw new NotImplementedException();
        }

        public async Task<BlobText> Get(string blobFileName)
        {
            throw new NotImplementedException();
        }
    }
}
