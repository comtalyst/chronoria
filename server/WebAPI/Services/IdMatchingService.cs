using Chronoria_WebAPI.Models;
using Chronoria_WebAPI.Repositories;

namespace Chronoria_WebAPI.Services
{
    public class IdMatchingService<DbContextType> : IIdMatchingService<DbContextType> where DbContextType : BaseContext
    {
        private readonly ICapsuleRepository<DbContextType> capsuleRepository;

        public IdMatchingService(ICapsuleRepository<DbContextType> capsuleRepository)
        {
            this.capsuleRepository = capsuleRepository;
        }
        public Task<bool> MatchReceipientEmail(string id, string receipientEmail)
        {
            throw new NotImplementedException();
        }
    }
}
