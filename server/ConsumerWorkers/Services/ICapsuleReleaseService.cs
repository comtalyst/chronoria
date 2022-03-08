namespace Chronoria_ConsumerWorkers.Services
{
    public interface ICapsuleReleaseService
    {
        public Task ReleaseRange(long timeL, long timeR);
        public Task ReleaseRange(DateTime timeL, DateTime timeR);
    }
}
