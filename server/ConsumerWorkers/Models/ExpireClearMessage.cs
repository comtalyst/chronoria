using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chronoria_ConsumerWorkers.Models
{
    public class ExpireClearMessage : IMessage
    {
        public long TimeL { get; set; }
        public long TimeR { get; set; }

        public ExpireClearMessage(string body)
        {
            var json = JToken.Parse(body);
            TimeL = long.Parse(json["TimeL"].ToString());
            TimeR = long.Parse(json["TimeR"].ToString());
        }
        public ExpireClearMessage(long TimeL, long TimeR)
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
                    MessageClass = "ExpireClearMessage",
                    Sender = "Chronoria-PersistentWorkers"
                },
                TimeL = TimeL,
                TimeR = TimeR
            };
            return JsonConvert.SerializeObject(obj);
        }
    }
}
