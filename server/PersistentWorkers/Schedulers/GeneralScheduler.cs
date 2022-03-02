using Chronoria_PersistentWorkers.utils;

namespace Chronoria_PersistentWorkers.Schedulers
{
    public abstract class GeneralScheduler : IScheduler
    {
        private long lastTime;
        private bool run=false;
        public async Task Start()
        {
            lastTime = await LastTime();
            run = true;
            _ = Looper();
        }

        public async Task Suspend()
        {
            run = false;
        }

        private async Task Looper()
        {
            while (run)
            {
                // Trigger using the time interval for parameters
                long curTime = TimeUtils.DateTimeToEpochMs(TimeUtils.now());
                await Trigger(lastTime + 1, curTime);

                // Update lastTime to current time
                lastTime = curTime;
                await SetLastTime(lastTime);

                // Calculate sleep time to be min(fetchTime, nextTime - curTime (newly defined for accuracy))
                long nextTime = await NextTime(curTime);
                long fetchTime = await FetchTime();
                long sleepTime;
                curTime = TimeUtils.DateTimeToEpochMs(TimeUtils.now());
                if (nextTime - curTime > fetchTime)
                {
                    sleepTime = fetchTime;
                }
                else
                {
                    sleepTime = nextTime - curTime;
                }
                if (sleepTime > 0)
                {
                    await Task.Delay((int)sleepTime);
                }
            }
        }

        protected abstract Task Trigger(long lastTime, long curTime);
        protected abstract Task<long> NextTime(long curTime);
        protected abstract Task<long> LastTime();
        protected abstract Task<long> FetchTime();
        protected abstract Task SetLastTime(long lastTime);
    }
}
