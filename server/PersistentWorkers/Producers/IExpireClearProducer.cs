using Chronoria_PersistentWorkers.Models;

namespace Chronoria_PersistentWorkers.Producers
{
    public interface IExpireClearProducer : IGeneralProducer<ExpireClearMessage> { }
}
