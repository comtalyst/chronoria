using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Producers
{
    public interface IGeneralProducer<MessageType> where MessageType : IMessage
    {
        public Task Produce(MessageType message);
        public Task Produce(IEnumerable<MessageType> messages);
    }
}
