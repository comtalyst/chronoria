using Chronoria_ConsumerWorkers.Models;
using Chronoria_ConsumerWorkers.Services;

namespace Chronoria_ConsumerWorkers.Consumers
{
    public class ActiveReceiptEmailConsumer : GeneralConsumer
    {
        private readonly IServiceProvider sp;
        private readonly IServiceScope scope;
        private readonly IActiveReceiptEmailService activeReceiptEmailService;

        public ActiveReceiptEmailConsumer(
            string connectionString,
            IServiceProvider sp
            ) : base(connectionString, "ActiveReceiptEmailTopic", "ConsumerWorkers-ActiveReceiptEmailTopic")
        {
            this.sp = sp;
            scope = this.sp.CreateScope();
            activeReceiptEmailService = scope.ServiceProvider.GetRequiredService<IActiveReceiptEmailService>();
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
                ActiveReceiptEmailMessage message = new ActiveReceiptEmailMessage(body);
                await activeReceiptEmailService.SendActiveReceiptEmail(message.Email, message.Ref, message.RecipientName, message.RecipientEmail, message.SendTime);
            }
            catch (Exception ex)
            {
                // TODO: make use of dead-lettering queue??
                Console.Error.WriteLine(ex);
            }
        }
    }
}
