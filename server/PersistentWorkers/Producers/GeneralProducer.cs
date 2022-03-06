using Azure.Messaging.ServiceBus;
using Chronoria_PersistentWorkers.Models;
using System;

namespace Chronoria_PersistentWorkers.Producers
{
    public abstract class GeneralProducer<MessageType> : IAsyncDisposable, IGeneralProducer<MessageType> where MessageType : IMessage
    {
        protected readonly string connectionString;
        protected readonly string topicName;
        protected readonly ServiceBusClient client;
        protected readonly ServiceBusSender sender;

        public GeneralProducer(string connectionString, string topicName)
        {
            this.connectionString = connectionString;
            this.topicName = topicName;
            client = new ServiceBusClient(connectionString);
            sender = client.CreateSender(topicName);
        }

        public async ValueTask DisposeAsync()
        {
            await sender.DisposeAsync();
            await client.DisposeAsync();
        }

        public async Task Produce(MessageType message)
        {
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
            if (!messageBatch.TryAddMessage(new ServiceBusMessage(message.Jsonify()))){
                throw new Exception($"The message is too large to fit in the batch.");
            }
            await sender.SendMessagesAsync(messageBatch);
        }
        public Task Produce(IEnumerable<MessageType> messages)
        {
            // TODO
            throw new NotImplementedException();
        }

    }
}
