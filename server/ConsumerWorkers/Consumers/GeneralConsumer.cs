using Chronoria_ConsumerWorkers.Models;
using Azure.Messaging.ServiceBus;

namespace Chronoria_ConsumerWorkers.Consumers
{
    public abstract class GeneralConsumer<MessageType> : BackgroundService, IAsyncDisposable, IConsumer<MessageType> where MessageType : IMessage
    {
        private readonly string connectionString;
        private readonly string topicName;
        private readonly string subscriptionName;
        private readonly ServiceBusClient client;
        private readonly ServiceBusProcessor processor;

        public GeneralConsumer(
            string connectionString,
            string topicName,
            string subscriptionName
            )
        {
            this.connectionString = connectionString;
            this.topicName = topicName;
            this.subscriptionName = subscriptionName;
            client = new ServiceBusClient(connectionString);
            processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());
            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;
        }
        public async ValueTask DisposeAsync()
        {
            await processor.DisposeAsync();
            await client.DisposeAsync();
        }

        public async Task Start()
        {
            await processor.StartProcessingAsync();
        }

        public async Task Suspend()
        {
            await processor.StopProcessingAsync();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await processor.StartProcessingAsync(stoppingToken);
        }
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await processor.StopProcessingAsync(stoppingToken);
            await base.StopAsync(stoppingToken);
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();          // TODO
            await args.CompleteMessageAsync(args.Message);      // commit
        }
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.Error.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
