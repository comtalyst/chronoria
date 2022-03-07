using Azure.Storage.Blobs;

namespace Chronoria_ConsumerWorkers.Repositories
{
    public interface IGeneralBlobRepository<BlobServiceClientType, ModelType> where BlobServiceClientType : BlobServiceClient where ModelType : class
    {
        Task<ModelType> Get(string blobFileName);
        Task<ModelType> Create(ModelType entry);
        Task Delete(string blobFileName);
        Uri GetTransferUri(string blobFileName);
        Task ReceiveTransfer(string blobFileName, Uri uri);
    }
}
