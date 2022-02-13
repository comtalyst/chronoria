using Newtonsoft.Json;

namespace Chronoria_WebAPI.Models
{
    public class ConfEmailMessage : IMessage
    {
        public string Email { get; set; }
        public string Ref { get; set; }

        public ConfEmailMessage(string Email, string Ref)
        {
            this.Email = Email;
            this.Ref = Ref;
        }
        public string Jsonify()
        {
            var obj = new
            {
                _meta = new {
                    MessageClass = "ConfEmailMessage",
                    Sender = "Chronoria-WebAPI"
                },
                Email = Email,
                Ref = Ref
            };
            return JsonConvert.SerializeObject(obj);
        }
    }
}
