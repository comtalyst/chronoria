namespace Chronoria_PersistentWorkers.Schedulers
{
    public interface IScheduler
    {
        public Task Start();
        public Task Suspend();
    }
}
