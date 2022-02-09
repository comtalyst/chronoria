using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Producers
{
    public class ConfEmailProducer : GeneralProducer<ConfEmailMessage>, IConfEmailProducer
    {
        public ConfEmailProducer(string connectionString) : base(connectionString, "ConfEmailTopic") { }
    }
}
