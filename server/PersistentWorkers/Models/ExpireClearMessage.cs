using Newtonsoft.Json;

namespace Chronoria_PersistentWorkers.Models
{
    public class ExpireClearMessage : IMessage
    {
        public long TimeL { get; set; }
        public long TimeR { get; set; }

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
