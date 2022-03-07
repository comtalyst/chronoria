using Chronoria_ConsumerWorkers.Models;

namespace Chronoria_ConsumerWorkers.Repositories
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
