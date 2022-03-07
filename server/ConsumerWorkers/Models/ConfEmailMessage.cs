using Newtonsoft.Json;

namespace Chronoria_ConsumerWorkers.Models
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
                    Sender = "Chronoria-ConsumerWorkers"
                },
                Email = Email,
                Ref = Ref
            };
            return JsonConvert.SerializeObject(obj);
        }
    }
}
