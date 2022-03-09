using Chronoria_ConsumerWorkers.Models;

namespace Chronoria_ConsumerWorkers.Repositories
{
    public interface ICapsuleRepository<DbContextType> : IGeneralRepository<Capsule> where DbContextType : BaseContext {
        public Task DeleteByCreateTimeRange(DateTime timeL, DateTime timeR);
        public Task<IEnumerable<Capsule>> GetBySendTimeRange(DateTime timeL, DateTime timeR);
    }
}
