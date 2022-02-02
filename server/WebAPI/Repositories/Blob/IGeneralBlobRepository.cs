using Azure.Storage.Blobs;

namespace Chronoria_WebAPI.Repositories.Blob
{
    public interface IGeneralBlobRepository<BlobServiceClientType, ModelType> where BlobServiceClientType : BlobServiceClient where ModelType : class
    {
        Task<ModelType> Get(int id);
        Task<ModelType> Create(ModelType entry);
        Task Delete(int id);
    }
}
