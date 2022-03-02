using Chronoria_PersistentWorkers.Models;

namespace Chronoria_PersistentWorkers.Producers
{
    public interface IGeneralProducer<MessageType> where MessageType : IMessage
    {
        public Task Produce(MessageType message);
        public Task Produce(IEnumerable<MessageType> messages);
    }
}
