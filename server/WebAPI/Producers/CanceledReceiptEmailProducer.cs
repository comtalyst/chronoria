using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Producers
{
    public class CanceledReceiptEmailProducer : GeneralProducer<CanceledReceiptEmailMessage>, ICanceledReceiptEmailProducer
    {
        public CanceledReceiptEmailProducer(string connectionString) : base(connectionString, "CanceledReceiptEmailTopic") { }
    }
}
