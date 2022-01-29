namespace Chronoria_WebAPI.Models
{
    public class FileContent
    {
        public string Id { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public int TextLocation { get; set; }
        public string TextFileId { get; set; }

        public FileContent(
            string Id,
            string FileId,
            string FileName,
            int TextLocation,
            string TextFileId
        )
        {
            this.Id = Id;
            this.FileId = FileId;
            this.FileName = FileName;
            this.TextLocation = TextLocation;
            this.TextFileId = TextFileId;
        }
    }
}
