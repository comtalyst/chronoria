using Chronoria_ConsumerWorkers.Models;

namespace Chronoria_ConsumerWorkers.Producers
{
    public class CapsuleDeliveryProducer : GeneralProducer<CapsuleDeliveryMessage>, ICapsuleDeliveryProducer
    {
        public CapsuleDeliveryProducer(string connectionString) : base(connectionString, "CapsuleDeliveryTopic") { }
    }
}
