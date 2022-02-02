using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Repositories
{
    public interface IGeneralRepository<DbContextType, ModelType> where DbContextType : BaseContext where ModelType : class
    {
        Task<IEnumerable<ModelType>> Get();
        Task<ModelType> Get(string id);
        Task<ModelType> Create(ModelType entry);
        Task Update(ModelType entry);
        Task Delete(string id);
    }
}
