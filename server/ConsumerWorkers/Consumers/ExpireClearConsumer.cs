namespace Chronoria_ConsumerWorkers.Consumers
{
    public class ExpireClearConsumer : GeneralConsumer
    {
        private readonly IServiceProvider sp;
        private readonly IServiceScope scope;

        public ExpireClearConsumer(
            string connectionString,
            IServiceProvider sp
            ) : base(connectionString, "TODO: topicName", "TODO: subscriptionName")
        {
            this.sp = sp;
            scope = this.sp.CreateScope();
        }
        public override async ValueTask DisposeAsync()
        {
            scope.Dispose();
            await base.DisposeAsync();
        }

        protected override Task ProcessMessage(string body)
        {
            throw new NotImplementedException();
        }
    }
}
