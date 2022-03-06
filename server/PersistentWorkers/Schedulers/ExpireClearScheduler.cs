using Chronoria_PersistentWorkers.Producers;
using Chronoria_PersistentWorkers.Repositories;
using Chronoria_PersistentWorkers.Models;
using Chronoria_PersistentWorkers.utils;

namespace Chronoria_PersistentWorkers.Schedulers
{
    public class ExpireClearScheduler : GeneralScheduler, IScheduler
    {
        private readonly long fetchTime;
        private IExpireClearProducer expireClearProducer;
        private ICapsuleRepository<PendingContext> pendingCapsuleRepository;
        private readonly IServiceProvider sp;

        public ExpireClearScheduler(long fetchTime, IServiceProvider sp)
        {
            this.fetchTime = fetchTime;
            this.sp = sp;
        }
        public ExpireClearScheduler(
            long fetchTime,
            IExpireClearProducer expireClearProducer,
            ICapsuleRepository<PendingContext> pendingCapsuleRepository
            )
        {
            this.fetchTime = fetchTime;
            this.expireClearProducer = expireClearProducer;
            this.pendingCapsuleRepository = pendingCapsuleRepository;
        }

        protected override long FetchTime()
        {
            return fetchTime;
        }

        protected override async Task<long> LastTime()
        {
            // TODO
            return 0;
        }

        protected override async Task LooperWrapper(CancellationToken token)
        {
            using (var scope = sp.CreateScope())
            {
                expireClearProducer = scope.ServiceProvider.GetRequiredService<IExpireClearProducer>();
                pendingCapsuleRepository = scope.ServiceProvider.GetRequiredService<ICapsuleRepository<PendingContext>>();
                await Looper(token);
            }
        }

        protected override async Task<long> NextTime(long curTime)
        {
            var nextCapsule = await pendingCapsuleRepository.GetNextByCreateTime(TimeUtils.EpochMsToDateTime(curTime-fetchTime));
            if(nextCapsule == null)
            {
                return long.MaxValue;
            }
            return TimeUtils.DateTimeToEpochMs(nextCapsule.CreateTime) + fetchTime;
        }

        protected override async Task SetLastTime(long lastTime)
        {
            // TODO
            return;
        }

        protected override async Task Trigger(long lastTime, long curTime)
        {
            Console.WriteLine("ExpireClearScheduler Triggered!");
            ExpireClearMessage expireClearMessage = new ExpireClearMessage(lastTime-fetchTime, curTime-fetchTime);
            await expireClearProducer.Produce(expireClearMessage);
        }
    }
}
