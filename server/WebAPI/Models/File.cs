using Microsoft.AspNetCore.Http;

namespace Chronoria_WebAPI.Models
{
    public class File
    {
        public string FileName { get; set; }
        public IFormFile FormFile { get; set; }
    }
}
