using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace Chronoria_WebAPI.Repositories
{
    public abstract class BlobRepositoryHelpers<BlobServiceClientType> where BlobServiceClientType : BlobServiceClient
    {
        protected readonly BlobServiceClientType _client;
        protected readonly string _containerName;
        protected readonly BlobContainerClient blobContainerClient;

        public BlobRepositoryHelpers(BlobServiceClientType client, string containerName)
        {
            _client = client;
            _containerName = containerName;
            blobContainerClient = _client.GetBlobContainerClient(containerName);
        }

        protected BlobClient GetClient(string blobFileName)
        {
            return blobContainerClient.GetBlobClient(blobFileName);
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
