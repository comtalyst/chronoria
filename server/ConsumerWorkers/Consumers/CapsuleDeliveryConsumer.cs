using Chronoria_ConsumerWorkers.Models;
using Chronoria_ConsumerWorkers.Services;

namespace Chronoria_ConsumerWorkers.Consumers
{
    public class CapsuleDeliveryConsumer : GeneralConsumer
    {
        private readonly IServiceProvider sp;
        private readonly IServiceScope scope;
        private readonly ICapsuleDeliveryService capsuleDeliveryService;

        public CapsuleDeliveryConsumer(
            string connectionString,
            IServiceProvider sp
            ) : base(connectionString, "CapsuleDeliveryTopic", "ConsumerWorkers-CapsuleDeliveryTopic")
        {
            this.sp = sp;
            scope = this.sp.CreateScope();
            capsuleDeliveryService = scope.ServiceProvider.GetRequiredService<ICapsuleDeliveryService>();
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
                CapsuleDeliveryMessage message = new CapsuleDeliveryMessage(body);
                if(message.contentType == ContentType.File)
                {
                    await capsuleDeliveryService.DeliverFile(
                        message.id,
                        message.senderEmail,
                        message.recipientEmail,
                        message.recipientEmail,
                        message.recipientName,
                        message.sendTime,
                        message.createTime,
                        message.text,
                        (Services.TextLocation)Enum.Parse(typeof(Services.TextLocation), message.textLocation.ToString()),      // TODO: fix
                        message.fileRef);
                }
                else if (message.contentType == ContentType.Text)
                {
                    await capsuleDeliveryService.DeliverText(
                        message.id,
                        message.senderEmail,
                        message.recipientEmail,
                        message.recipientEmail,
                        message.recipientName,
                        message.sendTime,
                        message.createTime,
                        message.text);
                }
                else
                {
                    throw new NotImplementedException("Unknown capsule type");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
    }
}
