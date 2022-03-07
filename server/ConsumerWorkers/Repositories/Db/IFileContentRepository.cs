using Chronoria_ConsumerWorkers.Models;

namespace Chronoria_ConsumerWorkers.Repositories
{
    public interface IFileContentRepository<DbContextType> : IGeneralRepository<FileContent> where DbContextType : BaseContext { }
}