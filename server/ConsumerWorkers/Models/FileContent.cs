namespace Chronoria_ConsumerWorkers.Models
{
    public enum TextLocation
    {
        Before,
        After
    }
    public class FileContent
    {
        public string Id { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public TextLocation TextLocation { get; set; }
        public string TextFileId { get; set; }

        public FileContent(
            string Id,
            string FileId,
            string FileName,
            TextLocation TextLocation,
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
