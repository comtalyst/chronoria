using Newtonsoft.Json;

namespace Chronoria_WebAPI.Models
{
    public class CanceledReceiptEmailMessage : IMessage
    {
        public string Email { get; set; }
        public string Ref { get; set; }
        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public long SendTime { get; set; }

        public CanceledReceiptEmailMessage(string Email, string Ref, string RecipientName, string RecipientEmail, long SendTime)
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
                    MessageClass = "CanceledReceiptEmailMessage",
                    Sender = "Chronoria-WebAPI"
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
