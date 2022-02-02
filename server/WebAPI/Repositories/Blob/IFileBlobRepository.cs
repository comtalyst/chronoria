using Azure.Storage.Blobs;
using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Repositories.Blob
{
    public interface IFileBlobRepository<BlobServiceClientType> : IGeneralBlobRepository<BlobServiceClientType, BlobFile> where BlobServiceClientType : BlobServiceClient { }
}
