using Chronoria_ConsumerWorkers.utils;
using Chronoria_ConsumerWorkers.Repositories;
using Chronoria_ConsumerWorkers.Models;

namespace Chronoria_ConsumerWorkers.Services
{
    public class ExpireClearService : IExpireClearService
    {
        private readonly ICapsuleRepository<PendingContext> pendingCapsuleRepository;

        public ExpireClearService(
            ICapsuleRepository<PendingContext> pendingCapsuleRepository
            )
        {
            this.pendingCapsuleRepository = pendingCapsuleRepository;
        }

        public async Task ClearRange(long timeL, long timeR)
        {
            await ClearRange(TimeUtils.EpochMsToDateTime(timeL), TimeUtils.EpochMsToDateTime(timeR));
        }
        public async Task ClearRange(DateTime timeL, DateTime timeR)
        {
            await pendingCapsuleRepository.DeleteByCreateTimeRange(timeL, timeR);
        }
    }
}
