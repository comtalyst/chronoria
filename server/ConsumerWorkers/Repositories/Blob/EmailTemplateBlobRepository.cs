using Azure.Storage.Blobs;
using Chronoria_ConsumerWorkers.Models;
using System.Text;

namespace Chronoria_ConsumerWorkers.Repositories
{
    public class EmailTemplateBlobRepository : BlobRepositoryHelpers<StaticBlobServiceClient>, IEmailTemplateBlobRepository
    {
        public EmailTemplateBlobRepository(StaticBlobServiceClient client, string containerName) : base(client, containerName) { }

        public async Task<BlobText> Create(BlobText entry)
        {
            throw new NotImplementedException();
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
