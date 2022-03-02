using Chronoria_PersistentWorkers.Models;

namespace Chronoria_PersistentWorkers.Repositories
{
    public interface ICapsuleRepository<DbContextType> : IGeneralRepository<Capsule> where DbContextType : BaseContext { }
}
