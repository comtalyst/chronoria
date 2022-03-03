using Chronoria_PersistentWorkers.Models;

namespace Chronoria_PersistentWorkers.Producers
{
    public class ExpireClearProducer : GeneralProducer<ExpireClearMessage>, IExpireClearProducer
    {
        public ExpireClearProducer(string connectionString) : base(connectionString, "ExpireClearTopic") { }
    }
}
