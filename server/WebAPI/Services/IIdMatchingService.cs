using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Services
{
    public interface IIdMatchingService<DbContextType> where DbContextType : BaseContext
    {
        public Task<bool> MatchReceipientEmail(string id, string receipientEmail);
    }
}
