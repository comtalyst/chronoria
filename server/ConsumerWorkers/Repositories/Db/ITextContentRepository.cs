using Chronoria_ConsumerWorkers.Models;

namespace Chronoria_ConsumerWorkers.Repositories
{
    public interface ITextContentRepository<DbContextType> : IGeneralRepository<TextContent> where DbContextType : BaseContext {}
}