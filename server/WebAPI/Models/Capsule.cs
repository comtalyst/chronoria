namespace Chronoria_WebAPI.Models
{
    public class Capsule
    {
        public string Id { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string ReceipentName { get; set; }
        public string ReceipentEmail { get; set; }
        public int ContentType { get; set; }
        public DateTime SendTime { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }

    }
}
