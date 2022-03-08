using Chronoria_ConsumerWorkers.Models;
using Chronoria_ConsumerWorkers.Services;

namespace Chronoria_ConsumerWorkers.Consumers
{
    public class ExpireClearConsumer : GeneralConsumer
    {
        private readonly IServiceProvider sp;
        private readonly IServiceScope scope;
        private readonly IExpireClearService expireClearService;

        public ExpireClearConsumer(
            string connectionString,
            IServiceProvider sp
            ) : base(connectionString, "ExpireClearTopic", "ConsumerWorkers-ExpireClearTopic")
        {
            this.sp = sp;
            scope = this.sp.CreateScope();
            expireClearService = scope.ServiceProvider.GetRequiredService<IExpireClearService>();
        }
        public override async ValueTask DisposeAsync()
        {
            scope.Dispose();
            await base.DisposeAsync();
        }

        protected override async Task ProcessMessage(string body)
        {
            try
            {
                ExpireClearMessage message = new ExpireClearMessage(body);
                await expireClearService.ClearRange(message.TimeL, message.TimeR);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
    }
}
