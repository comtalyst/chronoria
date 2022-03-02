namespace Chronoria_PersistentWorkers.Schedulers
{
    public class ExpireClearScheduler : GeneralScheduler, IScheduler
    {
        protected override Task<long> FetchTime()
        {
            throw new NotImplementedException();
        }

        protected override Task<long> LastTime()
        {
            throw new NotImplementedException();
        }

        protected override Task<long> NextTime(long curTime)
        {
            throw new NotImplementedException();
        }

        protected override Task SetLastTime(long lastTime)
        {
            throw new NotImplementedException();
        }

        protected override Task Trigger(long lastTime, long curTime)
        {
            throw new NotImplementedException();
        }
    }
}
