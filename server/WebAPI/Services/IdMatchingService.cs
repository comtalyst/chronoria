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
        public async Task<bool> MatchRecipientEmail(string id, string recipientEmail)
        {
            var capsule = await capsuleRepository.Get(id);
            if(capsule == null)
            {
                return false;
            }
            if(capsule.RecipientEmail != recipientEmail)
            {
                return false;
            }
            return true;
        }
    }
}
