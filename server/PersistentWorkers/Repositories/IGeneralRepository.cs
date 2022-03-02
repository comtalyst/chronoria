using Chronoria_PersistentWorkers.Models;

namespace Chronoria_PersistentWorkers.Repositories
{
    public interface IGeneralRepository<ModelType> where ModelType : class
    {
        Task<IEnumerable<ModelType>> Get();
        Task<ModelType> Get(string id);
        Task<ModelType> Create(ModelType entry);
        Task Update(ModelType entry);
        Task Delete(string id);
        Task<ModelType> Retrieve(string id);
    }
}
