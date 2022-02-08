using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Repositories
{
    public interface ITextContentRepository<DbContextType> : IGeneralRepository<TextContent> where DbContextType : BaseContext {}
}