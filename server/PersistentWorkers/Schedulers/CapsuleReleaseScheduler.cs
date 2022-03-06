using Chronoria_PersistentWorkers.Producers;
using Chronoria_PersistentWorkers.Repositories;
using Chronoria_PersistentWorkers.Models;
using Chronoria_PersistentWorkers.utils;

namespace Chronoria_PersistentWorkers.Schedulers
{
    public class CapsuleReleaseScheduler : GeneralScheduler, IScheduler
    {
        private readonly long fetchTime;
        private ICapsuleReleaseProducer capsuleReleaseProducer;
        private ICapsuleRepository<ActiveContext> activeCapsuleRepository;
        private readonly IServiceProvider sp;

        public CapsuleReleaseScheduler(long fetchTime, IServiceProvider sp)
        {
            this.fetchTime = fetchTime;
            this.sp = sp;
        }
        public CapsuleReleaseScheduler(
            long fetchTime,
            ICapsuleReleaseProducer capsuleReleaseProducer,
            ICapsuleRepository<ActiveContext> activeCapsuleRepository
            )
        {
            this.fetchTime = fetchTime;
            this.capsuleReleaseProducer = capsuleReleaseProducer;
            this.activeCapsuleRepository = activeCapsuleRepository;
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
                capsuleReleaseProducer = scope.ServiceProvider.GetRequiredService<ICapsuleReleaseProducer>();
                activeCapsuleRepository = scope.ServiceProvider.GetRequiredService<ICapsuleRepository<ActiveContext>>();
                await Looper(token);
            }
        }

        protected override async Task<long> NextTime(long curTime)
        {
            var nextCapsule = await activeCapsuleRepository.GetNextBySendTime(TimeUtils.EpochMsToDateTime(curTime));
            if (nextCapsule == null)
            {
                return long.MaxValue;
            }
            return TimeUtils.DateTimeToEpochMs(nextCapsule.SendTime);
        }

        protected override async Task SetLastTime(long lastTime)
        {
            // TODO
            return;
        }

        protected override async Task Trigger(long lastTime, long curTime)
        {
            CapsuleReleaseMessage capsuleReleaseMessage = new CapsuleReleaseMessage(lastTime, curTime);
            await capsuleReleaseProducer.Produce(capsuleReleaseMessage);
        }
    }
}
