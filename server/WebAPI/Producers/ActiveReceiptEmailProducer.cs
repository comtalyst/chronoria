using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Producers
{
    public class ActiveReceiptEmailProducer : GeneralProducer<ActiveReceiptEmailMessage>, IActiveReceiptEmailProducer
    {
        public ActiveReceiptEmailProducer(string connectionString) : base(connectionString, "ActiveReceiptEmailTopic") { }
    }
}
