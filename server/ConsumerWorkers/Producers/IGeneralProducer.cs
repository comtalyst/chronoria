using Chronoria_ConsumerWorkers.Models;

namespace Chronoria_ConsumerWorkers.Producers
{
    public interface IGeneralProducer<MessageType> where MessageType : IMessage
    {
        public Task Produce(MessageType message);
        public Task Produce(IEnumerable<MessageType> messages);
    }
}
