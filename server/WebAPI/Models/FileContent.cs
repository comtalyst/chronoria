namespace Chronoria_WebAPI.Models
{
    public class FileContent
    {
        public string Id { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public int TextLocation { get; set; }
        public string TextFileId { get; set; }
    }
}
