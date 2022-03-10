using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chronoria_ConsumerWorkers.Models
{
    public class CapsuleReleaseMessage : IMessage
    {
        public long TimeL { get; set; }
        public long TimeR { get; set; }

        public CapsuleReleaseMessage(string body)
        {
            var json = JToken.Parse(body);
            TimeL = long.Parse(json["TimeL"].ToString());
            TimeR = long.Parse(json["TimeR"].ToString());
        }
        public CapsuleReleaseMessage(long TimeL, long TimeR)
        {
            this.TimeL = TimeL;
            this.TimeR = TimeR;
        }
        public string Jsonify()
        {
            var obj = new
            {
                _meta = new
                {
                    MessageClass = "CapsuleReleaseMessage",
                    Sender = "Chronoria-PersistentWorkers"
                },
                TimeL = TimeL,
                TimeR = TimeR
            };
            return JsonConvert.SerializeObject(obj);
        }
    }
}
