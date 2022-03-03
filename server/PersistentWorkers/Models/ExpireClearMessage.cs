using Newtonsoft.Json;

namespace Chronoria_PersistentWorkers.Models
{
    public class ExpireClearMessage : IMessage
    {
        public DateTime TimeL { get; set; }
        public DateTime TimeR { get; set; }

        public ExpireClearMessage(DateTime TimeL, DateTime TimeR)
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
