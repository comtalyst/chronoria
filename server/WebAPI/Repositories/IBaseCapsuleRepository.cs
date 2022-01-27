using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Repositories
{
    public interface IBaseCapsuleRepository
    {
        Task<IEnumerable<Capsule>> Get();
        Task<Capsule> Get(int id);
        Task<Capsule> Create(Capsule capsule);
        Task Update(Capsule capsule);
        Task Delete(int id);
    }
}
