using Azure.Storage.Blobs;
using Chronoria_ConsumerWorkers.Models;
using System.Text;

namespace Chronoria_ConsumerWorkers.Repositories
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

        public async Task<BlobText> Get(string blobFileName)
        {
            BlobClient blobClient = GetClient(blobFileName);
            var stream = await blobClient.OpenReadAsync();
            StreamReader reader = new StreamReader(stream);
            BlobText blobText = new BlobText(blobFileName, reader.ReadToEnd());
            return blobText;
        }
    }
}
