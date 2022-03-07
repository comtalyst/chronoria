using Newtonsoft.Json;

namespace Chronoria_ConsumerWorkers.Models
{
    public class ExpireClearMessage : IMessage
    {
        public long TimeL { get; set; }
        public long TimeR { get; set; }

        public ExpireClearMessage(string body)
        {
            var obj = JsonConvert.DeserializeObject<ExpireClearMessage>(body);
            TimeL = obj.TimeL;
            TimeR = obj.TimeR;
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
