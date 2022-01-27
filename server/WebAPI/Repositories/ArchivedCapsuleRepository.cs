using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Repositories
{
    public class ArchivedCapsuleRepository : BaseCapsuleRepository, IArchivedCapsuleRepository
    {
        public ArchivedCapsuleRepository(ArchivedContext context) : base(context) { }
    }
}
