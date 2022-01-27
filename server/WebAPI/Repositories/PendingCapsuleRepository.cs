using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Repositories
{
    public class PendingCapsuleRepository : BaseCapsuleRepository, IPendingCapsuleRepository
    {
        public PendingCapsuleRepository(PendingContext context) : base(context) { }
    }
}
