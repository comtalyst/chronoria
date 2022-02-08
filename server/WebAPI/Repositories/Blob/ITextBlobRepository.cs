using Azure.Storage.Blobs;
using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Repositories
{
    public interface ITextBlobRepository<BlobServiceClientType> : IGeneralBlobRepository<BlobServiceClientType, BlobText> where BlobServiceClientType : BlobServiceClient { }
}
