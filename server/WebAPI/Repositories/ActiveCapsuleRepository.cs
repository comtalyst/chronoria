using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Repositories
{
    public class ActiveCapsuleRepository : BaseCapsuleRepository, IActiveCapsuleRepository
    {
        public ActiveCapsuleRepository(ActiveContext context) : base(context) { }
    }
}
