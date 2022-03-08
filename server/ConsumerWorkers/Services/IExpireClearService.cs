namespace Chronoria_ConsumerWorkers.Services
{
    public interface IExpireClearService
    {
        public Task ClearRange(long timeL, long timeR);
        public Task ClearRange(DateTime timeL, DateTime timeR);
    }
}
