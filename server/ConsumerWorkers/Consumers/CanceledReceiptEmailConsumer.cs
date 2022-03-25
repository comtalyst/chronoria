using Chronoria_ConsumerWorkers.Models;
using Chronoria_ConsumerWorkers.Services;

namespace Chronoria_ConsumerWorkers.Consumers
{
    public class CanceledReceiptEmailConsumer : GeneralConsumer
    {
        private readonly IServiceProvider sp;
        private readonly IServiceScope scope;
        private readonly ICanceledReceiptEmailService canceledReceiptEmailService;

        public CanceledReceiptEmailConsumer(
            string connectionString,
            IServiceProvider sp
            ) : base(connectionString, "CanceledReceiptEmailTopic", "ConsumerWorkers-CanceledReceiptEmailTopic")
        {
            this.sp = sp;
            scope = this.sp.CreateScope();
            canceledReceiptEmailService = scope.ServiceProvider.GetRequiredService<ICanceledReceiptEmailService>();
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
                CanceledReceiptEmailMessage message = new CanceledReceiptEmailMessage(body);
                await canceledReceiptEmailService.SendCanceledReceiptEmail(message.Email, message.RecipientName, message.RecipientEmail, message.SendTime);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
    }
}
