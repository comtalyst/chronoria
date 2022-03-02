using Microsoft.EntityFrameworkCore;

namespace Chronoria_PersistentWorkers.Models
{
    public class PendingContext : BaseContext
    {
        public PendingContext(DbContextOptions<PendingContext> options) : base(options) { }
    }
}
