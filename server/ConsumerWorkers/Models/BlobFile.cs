using Microsoft.AspNetCore.Http;

namespace Chronoria_ConsumerWorkers.Models
{
    public class BlobFile
    {
        public string BlobFileName { get; set; }
        public IFormFile FormFile { get; set; }

        public BlobFile(string BlobFileName, IFormFile FormFile)
        {
            this.BlobFileName = BlobFileName;
            this.FormFile = FormFile;
        }
    }
}
