using Chronoria_PersistentWorkers.utils;

namespace Chronoria_PersistentWorkers.Schedulers
{
    public abstract class GeneralScheduler : IScheduler
    {
        private long lastTime;
        private Task looperTask;
        private CancellationTokenSource tokenSource;

        public async Task Start()
        {
            tokenSource = new CancellationTokenSource();
            lastTime = await LastTime();
            looperTask = Task.Run(() => Looper(tokenSource.Token), tokenSource.Token);
        }

        public async Task Suspend()
        {
            tokenSource.Cancel();
            try
            {
                await looperTask;
            }
            catch (OperationCanceledException e)
            {
                
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                //throw e;
            }
            finally
            {
                tokenSource.Dispose();
            }

        }

        private async Task Looper(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // Trigger using the time interval for parameters
                long curTime = TimeUtils.DateTimeToEpochMs(TimeUtils.now());
                await Trigger(lastTime + 1, curTime);

                // Update lastTime to current time
                lastTime = curTime;
                await SetLastTime(lastTime);

                // Calculate sleep time to be min(fetchTime, nextTime - curTime (newly defined for accuracy))
                long nextTime = await NextTime(curTime);
                if(nextTime == curTime)
                {
                    //throw new Exception("Whattt nextTime = curTime");
                    nextTime = await NextTime(curTime + 1);                     // temporary
                }
                long fetchTime = FetchTime();
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
                    long wakeTime = curTime + sleepTime;
                    await Task.Delay((int)sleepTime, token);
                    token.ThrowIfCancellationRequested();

                    // penalty for inaccuracy of Task.Delay()
                    curTime = TimeUtils.DateTimeToEpochMs(TimeUtils.now());
                    while (curTime < wakeTime)
                    {
                        await Task.Delay((int)(wakeTime - curTime), token);
                        token.ThrowIfCancellationRequested();
                        curTime = TimeUtils.DateTimeToEpochMs(TimeUtils.now());
                    }
                }
            }
            token.ThrowIfCancellationRequested();
        }

        protected abstract Task Trigger(long lastTime, long curTime);
        protected abstract Task<long> NextTime(long curTime);
        protected abstract Task<long> LastTime();
        protected abstract long FetchTime();
        protected abstract Task SetLastTime(long lastTime);
    }
}
