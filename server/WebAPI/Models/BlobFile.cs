using Microsoft.AspNetCore.Http;

namespace Chronoria_WebAPI.Models
{
    public class BlobFile
    {
        public string BlobFileName { get; set; }
        public IFormFile FormFile { get; set; }
    }
}
