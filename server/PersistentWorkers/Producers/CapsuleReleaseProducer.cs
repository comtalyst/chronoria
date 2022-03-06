using Chronoria_PersistentWorkers.Models;

namespace Chronoria_PersistentWorkers.Producers
{
    public class CapsuleReleaseProducer : GeneralProducer<CapsuleReleaseMessage>, ICapsuleReleaseProducer
    {
        public CapsuleReleaseProducer(string connectionString) : base(connectionString, "CapsuleReleaseTopic") { }
    }
}
