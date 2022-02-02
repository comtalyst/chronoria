using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Repositories.Blob
{
    public class FileBlobRepository<BlobServiceClientType> : IFileBlobRepository<BlobServiceClientType> where BlobServiceClientType : BlobServiceClient
    {
        protected readonly BlobServiceClientType _client;
        protected readonly string _containerName;
        protected readonly BlobContainerClient blobContainerClient;

        public FileBlobRepository(BlobServiceClientType client, string containerName)
        {
            _client = client;
            _containerName = containerName;
            blobContainerClient = _client.CreateBlobContainer(containerName);
        }

        private BlobClient GetClient(string blobFileName)
        {
            return blobContainerClient.GetBlobClient(blobFileName);
        }

        public async Task<BlobFile> Create(BlobFile entry)
        {
            BlobClient blobClient = GetClient(entry.BlobFileName);
            using(var stream = entry.FormFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream);
            }
            return entry;
        }

        public async Task Delete(string blobFileName)
        {
            throw new NotImplementedException();
        }

        public async Task<BlobFile> Get(string blobFileName)
        {
            throw new NotImplementedException();
        }

        public Uri GetTransferUri(string blobFileName)
        {
            DateTimeOffset expiredOn = DateTimeOffset.UtcNow.AddMinutes(60);            // TODO
            BlobClient blobClient = GetClient(blobFileName);
            Uri uri = blobClient.GenerateSasUri(BlobSasPermissions.Read, expiredOn);
            return uri;
        }

        public async Task ReceiveTransfer(string blobFileName, Uri uri)
        {
            BlobClient blobClient = GetClient(blobFileName);
            CopyFromUriOperation ops = await blobClient.StartCopyFromUriAsync(uri);

            // check progress
            BlobProperties properties = (await blobClient.GetPropertiesAsync()).Value;
            while (!ops.HasCompleted)
            {
                long copied = await ops.WaitForCompletionAsync();

                Console.WriteLine($"Blob: {blobFileName}, Copied: {copied} of {properties.ContentLength}");
                await Task.Delay(500);          // TODO
            }
        }
    }
}
