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

        public Uri? GetTransferUri(string blobFileName)
        {
            try
            {
                DateTimeOffset expiredOn = DateTimeOffset.UtcNow.AddMinutes(60);            // TODO: fix this by passing config provider?, but has to be done on all implemented services (constructor)
                BlobClient blobClient = GetClient(blobFileName);
                Uri uri = blobClient.GenerateSasUri(BlobSasPermissions.Read, expiredOn);
                return uri;
            }
            catch (Exception ex)
            {
                return null;
            }
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

        public async Task Delete(string blobFileName)
        {
            BlobClient blobClient = GetClient(blobFileName);
            await blobClient.DeleteIfExistsAsync();
        }

        public string GetDownloadLink(string blobFileName, string downloadFileName)
        {
            BlobClient blobClient = GetClient(blobFileName);
            BlobSasBuilder blobSasBuilder = new BlobSasBuilder(
                BlobSasPermissions.Read,
                DateTimeOffset.UtcNow.AddHours(12));                // TODO
            blobSasBuilder.ContentDisposition = "attachment; filename=" + downloadFileName;
            return blobClient.GenerateSasUri(blobSasBuilder).ToString();
        }
    }
}
