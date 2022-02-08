using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Repositories
{
    public interface IFileContentRepository<DbContextType> : IGeneralRepository<FileContent> where DbContextType : BaseContext { }
}