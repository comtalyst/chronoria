using Azure.Storage.Blobs;
using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Repositories.Blob
{
    public class FileBlobRepository<BlobServiceClientType> : IFileBlobRepository<BlobServiceClientType> where BlobServiceClientType : BlobServiceClient
    {
        protected readonly BlobServiceClientType _client;
        protected readonly string _containerName;

        public FileBlobRepository(BlobServiceClientType client, string containerName)
        {
            _client = client;
            _containerName = containerName;
        }

        public Task<BlobFile> Create(BlobFile entry)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BlobFile> Get(int id)
        {
            throw new NotImplementedException();
        }
    }
    {
    }
}
