using Azure.Storage.Blobs;

namespace Chronoria_WebAPI.Repositories
{
    public interface IGeneralBlobRepository<BlobServiceClientType, ModelType> where BlobServiceClientType : BlobServiceClient where ModelType : class
    {
        Task<ModelType> Get(string blobFileName);
        Task<ModelType> Create(ModelType entry);
        Task Delete(string blobFileName);
    }
}
