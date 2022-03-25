using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chronoria_ConsumerWorkers.Models
{
    public class ActiveReceiptEmailMessage : IMessage
    {
        public string Email { get; set; }
        public string Ref { get; set; }
        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public long SendTime { get; set; }

        public ActiveReceiptEmailMessage(string body)
        {
            var json = JToken.Parse(body);
            Email = json["Email"].ToString();
            Ref = json["Ref"].ToString();
            RecipientName = json["RecipientName"].ToString();
            RecipientEmail = json["RecipientEmail"].ToString();
            SendTime = long.Parse(json["SendTime"].ToString());
        }
        public ActiveReceiptEmailMessage(string Email, string Ref, string RecipientName, string RecipientEmail, long SendTime)
        {
            this.Email = Email;
            this.Ref = Ref;
            this.RecipientName = RecipientName;
            this.RecipientEmail = RecipientEmail;
            this.SendTime = SendTime;
        }
        public string Jsonify()
        {
            var obj = new
            {
                _meta = new                                         // TODO: share this meta code on some parental level
                {
                    MessageClass = "ActiveReceiptEmailMessage",
                    Sender = "Chronoria-ConsumerWorkers"
                },
                Email = Email,
                Ref = Ref,
                RecipientName = RecipientName,
                RecipientEmail = RecipientEmail,
                SendTime = SendTime
            };
            return JsonConvert.SerializeObject(obj);
        }
    }
}
