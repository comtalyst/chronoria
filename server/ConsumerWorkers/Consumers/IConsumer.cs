using Chronoria_ConsumerWorkers.Models;

namespace Chronoria_ConsumerWorkers.Consumers
{
    public interface IConsumer<MessageType> where MessageType : IMessage
    {
        public Task Start();
        public Task Suspend();
    }
}
