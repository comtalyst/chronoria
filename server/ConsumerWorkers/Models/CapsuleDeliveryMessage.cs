using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chronoria_ConsumerWorkers.Models
{
    public class CapsuleDeliveryMessage : IMessage
    {
        public string id { get; set; }
        public string senderEmail { get; set; }
        public string senderName { get; set; }
        public string recipientEmail { get; set; }
        public string recipientName { get; set; }
        public long sendTime { get; set; }
        public long createTime { get; set; }
        public ContentType contentType { get; set; }
        public string text { get; set; }
        public TextLocation textLocation { get; set; }
        public string fileRef { get; set; }

        public CapsuleDeliveryMessage(string body)
        {
            var json = JToken.Parse(body);
            id = json["id"].ToString();
            senderEmail = json["senderEmail"].ToString();
            senderName = json["senderName"].ToString();
            recipientEmail = json["recipientEmail"].ToString();
            recipientName = json["recipientName"].ToString();
            sendTime = long.Parse(json["sendTime"].ToString());
            createTime = long.Parse(json["createTime"].ToString());

            text = json["content"]["text"].ToString();
            if(json["content"]["fileRef"] != null)
            {
                contentType = ContentType.File;
                try
                {
                    // TODO: clean this up
                    // will be deprecated
                    fileRef = json["content"]["fileRef"].ToString();
                    textLocation = (TextLocation)Enum.Parse(typeof(TextLocation), json["content"]["textLocation"].ToString());
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
            }
            else
            {
                contentType = ContentType.Text;
            }
        }
        public CapsuleDeliveryMessage(
            string id,
            string senderEmail,
            string senderName,
            string recipientEmail,
            string recipientName,
            long sendTime,
            long createTime,
            string text
            )
        {
            this.contentType = ContentType.Text;
            this.id = id;
            this.senderEmail = senderEmail;
            this.senderName = senderName;
            this.recipientEmail = recipientEmail;
            this.recipientName = recipientName;
            this.sendTime = sendTime;
            this.createTime = createTime;
            this.text = text;
        }
        public CapsuleDeliveryMessage(
            string id,
            string senderEmail,
            string senderName,
            string recipientEmail,
            string recipientName,
            long sendTime,
            long createTime,
            string text,
            TextLocation textLocation,
            string fileRef
            )
        {
            this.contentType = ContentType.File;
            this.id = id;
            this.senderEmail = senderEmail;
            this.senderName = senderName;
            this.recipientEmail = recipientEmail;
            this.recipientName = recipientName;
            this.sendTime = sendTime;
            this.createTime = createTime;
            this.text = text;
            this.textLocation = textLocation;
            this.fileRef = fileRef;
        }
        public string Jsonify()
        {
            if(contentType == ContentType.File)
            {
                var obj = new
                {
                    _meta = new
                    {
                        MessageClass = "CapsuleDeliveryMessage",
                        Sender = "Chronoria-PersistentWorkers"
                    },
                    id = id,
                    senderEmail = senderEmail,
                    senderName = senderName,
                    recipientEmail = recipientEmail,
                    recipientName = recipientName,
                    sendTime = sendTime,
                    createTime = createTime,
                    content = new
                    {
                        text = text,
                        textLocation = textLocation,
                        fileRef = fileRef
                    }
                };
                return JsonConvert.SerializeObject(obj);
            }
            else if(contentType == ContentType.Text)
            {
                var obj = new
                {
                    _meta = new
                    {
                        MessageClass = "CapsuleDeliveryMessage",
                        Sender = "Chronoria-PersistentWorkers"
                    },
                    id = id,
                    senderEmail = senderEmail,
                    senderName = senderName,
                    recipientEmail = recipientEmail,
                    recipientName = recipientName,
                    sendTime = sendTime,
                    createTime = createTime,
                    content = new
                    {
                        text = text
                    }
                };
                return JsonConvert.SerializeObject(obj);
            }
            else
            {
                throw new NotImplementedException("Unknown capsule type");
            }
        }
    }
}
