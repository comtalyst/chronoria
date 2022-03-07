namespace Chronoria_ConsumerWorkers.Models
{
    public class BlobText
    {
        public string BlobFileName { get; set; }
        public string content { get; set; }

        public BlobText(string BlobFileName, string content)
        {
            this.BlobFileName = BlobFileName;
            this.content = content;
        }
    }
}
