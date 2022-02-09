namespace Chronoria_WebAPI.Models
{
    public enum ContentType
    {
        File,
        Text
    }
    public enum Status
    {
        Pending,
        Active,
        Archived
    }
    public class Capsule
    {
        public string Id { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientName { get; set; }
        public ContentType ContentType { get; set; }
        public DateTime SendTime { get; set; }
        public DateTime CreateTime { get; set; }
        public Status Status { get; set; }

        public Capsule(
            string Id, 
            string SenderEmail, 
            string SenderName, 
            string RecipientEmail,
            string RecipientName,
            ContentType ContentType,
            DateTime SendTime,
            DateTime CreateTime,
            Status status
        )
        {
            this.Id = Id;
            this.SenderEmail = SenderEmail;
            this.SenderName = SenderName;
            this.RecipientEmail = RecipientEmail;
            this.RecipientName = RecipientName;
            this.ContentType = ContentType;
            this.SendTime = SendTime;
            this.CreateTime = CreateTime;
            this.Status = status;
        }
    }
}
