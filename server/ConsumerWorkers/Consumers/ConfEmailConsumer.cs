using Chronoria_ConsumerWorkers.Models;
using Chronoria_ConsumerWorkers.Services;

namespace Chronoria_ConsumerWorkers.Consumers
{
    public class ConfEmailConsumer : GeneralConsumer
    {
        private readonly IServiceProvider sp;
        private readonly IServiceScope scope;
        private readonly IConfEmailService confEmailService;

        public ConfEmailConsumer(
            string connectionString,
            IServiceProvider sp
            ) : base(connectionString, "ConfEmailTopic", "ConsumerWorkers-ConfEmailTopic")
        {
            this.sp = sp;
            scope = this.sp.CreateScope();
            confEmailService = scope.ServiceProvider.GetRequiredService<IConfEmailService>();
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
                ConfEmailMessage message = new ConfEmailMessage(body);
                await confEmailService.SendConfEmail(message.Email, message.Ref);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
    }
}
