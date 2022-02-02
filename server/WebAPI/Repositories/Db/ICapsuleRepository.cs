using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Repositories
{
    public interface ICapsuleRepository<DbContextType> : IGeneralRepository<DbContextType, Capsule> where DbContextType : BaseContext { }
}
