using Azure.Storage.Blobs;
using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Repositories
{
    public interface IFileBlobRepository<BlobServiceClientType> : IGeneralBlobRepository<BlobServiceClientType, BlobFile> where BlobServiceClientType : BlobServiceClient
    {
        public string GetDownloadLink(string blobFileName, string downloadFileName);
    }
}
