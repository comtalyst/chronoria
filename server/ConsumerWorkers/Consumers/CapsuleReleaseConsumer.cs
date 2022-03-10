using Chronoria_ConsumerWorkers.Models;
using Chronoria_ConsumerWorkers.Services;

namespace Chronoria_ConsumerWorkers.Consumers
{
    public class CapsuleReleaseConsumer : GeneralConsumer
    {
        private readonly IServiceProvider sp;
        private readonly IServiceScope scope;
        private readonly ICapsuleReleaseService capsuleReleaseService;

        public CapsuleReleaseConsumer(
            string connectionString,
            IServiceProvider sp
            ) : base(connectionString, "CapsuleReleaseTopic", "ConsumerWorkers-CapsuleReleaseTopic")
        {
            this.sp = sp;
            scope = this.sp.CreateScope();
            capsuleReleaseService = scope.ServiceProvider.GetRequiredService<ICapsuleReleaseService>();
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
                CapsuleReleaseMessage message = new CapsuleReleaseMessage(body);
                await capsuleReleaseService.ReleaseRange(message.TimeL, message.TimeR);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
    }
}
