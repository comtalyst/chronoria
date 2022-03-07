using Chronoria_ConsumerWorkers.Models;
using Azure.Messaging.ServiceBus;

namespace Chronoria_ConsumerWorkers.Consumers
{
    public abstract class GeneralConsumer : BackgroundService, IAsyncDisposable, IConsumer
    {
        private readonly string connectionString;
        private readonly string topicName;
        private readonly string subscriptionName;
        private readonly ServiceBusClient client;
        private readonly ServiceBusProcessor processor;
        private readonly IServiceProvider sp;
        protected readonly IServiceScope scope;

        public GeneralConsumer(
            string connectionString,
            string topicName,
            string subscriptionName,
            IServiceProvider sp
            )
        {
            this.connectionString = connectionString;
            this.topicName = topicName;
            this.subscriptionName = subscriptionName;
            this.sp = sp;

            client = new ServiceBusClient(this.connectionString);
            processor = client.CreateProcessor(this.topicName, this.subscriptionName, new ServiceBusProcessorOptions());
            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            scope = sp.CreateScope();
        }
        public async ValueTask DisposeAsync()
        {
            scope.Dispose();
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

        protected abstract Task ProcessMessage(string body);

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            try
            {
                await ProcessMessage(body);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
            await args.CompleteMessageAsync(args.Message);      // commit
        }
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.Error.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
