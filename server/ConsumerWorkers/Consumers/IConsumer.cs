using Chronoria_ConsumerWorkers.Models;

namespace Chronoria_ConsumerWorkers.Consumers
{
    public interface IConsumer
    {
        public Task Start();
        public Task Suspend();
    }
}
