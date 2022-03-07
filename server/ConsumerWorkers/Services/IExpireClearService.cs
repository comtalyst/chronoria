namespace Chronoria_ConsumerWorkers.Services
{
    public interface IExpireClearService
    {
        public Task ClearRange(long TimeL, long TimeR);
        public Task ClearRange(DateTime TimeL, DateTime TimeR);
    }
}
