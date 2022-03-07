using Chronoria_PersistentWorkers.utils;

namespace Chronoria_PersistentWorkers.Schedulers
{
    public abstract class GeneralScheduler : BackgroundService, IScheduler
    {
        private long lastTime;
        private Task looperTask;

        private CancellationTokenSource tokenSource;

        // for testing
        public async Task Start()
        {
            tokenSource = new CancellationTokenSource();
            await ExecuteAsync(tokenSource.Token);
        }
        public async Task Suspend()
        {
            tokenSource.Cancel();
            try
            {
                await looperTask;
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }
            tokenSource.Dispose();
        }

        // for integration
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            lastTime = await LastTime();
            looperTask = Task.Run(() => LooperWrapper(stoppingToken), stoppingToken);
        }
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            try
            {
                await looperTask;
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }
            await base.StopAsync(stoppingToken);
        }

        protected async Task Looper(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // Trigger using the time interval for parameters
                long curTime = TimeUtils.DateTimeToEpochMs(TimeUtils.now());
                try
                {
                    await Trigger(lastTime + 1, curTime);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.ToString());
                }

                // Update lastTime to current time
                lastTime = curTime;
                try
                {
                    await SetLastTime(lastTime);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.ToString());
                }

                // Calculate sleep time to be min(fetchTime, nextTime - curTime (newly defined for accuracy))
                long nextTime = long.MaxValue;
                try
                {
                    nextTime = await NextTime(curTime);
                    if (nextTime == curTime)
                    {
                        nextTime = await NextTime(curTime + 1);                     // temporary
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.ToString());
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
        protected abstract Task LooperWrapper(CancellationToken token);
    }
}
