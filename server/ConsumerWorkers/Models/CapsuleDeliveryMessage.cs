using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chronoria_ConsumerWorkers.Models
{
    public class CapsuleDeliveryMessage : IMessage
    {
        public string id { get; set; }
        public string senderEmail { get; set; }
        public string senderName { get; set; }
        public string receipientEmail { get; set; }
        public string receipientName { get; set; }
        public long sendTime { get; set; }
        public long createTime { get; set; }
        public ContentType contentType { get; set; }
        public string text { get; set; }
        public TextLocation textLocation { get; set; }
        public string fileRef { get; set; }

        public CapsuleDeliveryMessage(string body)
        {
            // TODO
            var json = JToken.Parse(body);
            //TimeL = long.Parse(json["TimeL"].ToString());
            //TimeR = long.Parse(json["TimeR"].ToString());
        }
        public CapsuleDeliveryMessage(
            string id,
            string senderEmail,
            string senderName,
            string receipientEmail,
            string receipientName,
            long sendTime,
            long createTime,
            string text
            )
        {
            this.contentType = ContentType.Text;
            this.id = id;
            this.senderEmail = senderEmail;
            this.senderName = senderName;
            this.receipientEmail = receipientEmail;
            this.receipientName = receipientName;
            this.sendTime = sendTime;
            this.createTime = createTime;
            this.text = text;
        }
        public CapsuleDeliveryMessage(
            string id,
            string senderEmail,
            string senderName,
            string receipientEmail,
            string receipientName,
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
            this.receipientEmail = receipientEmail;
            this.receipientName = receipientName;
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
                    receipientEmail = receipientEmail,
                    receipientName = receipientName,
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
                    receipientEmail = receipientEmail,
                    receipientName = receipientName,
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
