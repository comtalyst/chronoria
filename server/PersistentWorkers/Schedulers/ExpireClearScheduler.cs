using Chronoria_PersistentWorkers.Producers;
using Chronoria_PersistentWorkers.Repositories;
using Chronoria_PersistentWorkers.Models;
using Chronoria_PersistentWorkers.utils;

namespace Chronoria_PersistentWorkers.Schedulers
{
    public class ExpireClearScheduler : GeneralScheduler, IScheduler
    {
        private readonly long fetchTime;
        private readonly IExpireClearProducer expireClearProducer;
        private readonly ICapsuleRepository<PendingContext> pendingCapsuleRepository;

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

        protected override async Task<long> NextTime(long curTime)
        {
            var nextCapsule = await pendingCapsuleRepository.GetNextByCreateTime(TimeUtils.EpochMsToDateTime(curTime));
            if(nextCapsule == null)
            {
                return long.MaxValue;
            }
            return TimeUtils.DateTimeToEpochMs(nextCapsule.CreateTime);
        }

        protected override async Task SetLastTime(long lastTime)
        {
            // TODO
            return;
        }

        protected override async Task Trigger(long lastTime, long curTime)
        {
            ExpireClearMessage expireClearMessage = new ExpireClearMessage(lastTime, curTime);
            await expireClearProducer.Produce(expireClearMessage);
        }
    }
}
