namespace Chronoria_WebAPI.Models
{
    public class TextContent
    {
        public string Id { get; set; }
        public string TextFileId { get; set; }

        public TextContent(
            string Id,
            string TextFileId
        )
        {
            this.Id = Id;
            this.TextFileId = TextFileId;
        }
    }
}
