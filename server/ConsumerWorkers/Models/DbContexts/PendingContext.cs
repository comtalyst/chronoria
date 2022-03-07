using Microsoft.EntityFrameworkCore;

namespace Chronoria_ConsumerWorkers.Models
{
    public class PendingContext : BaseContext
    {
        public PendingContext(DbContextOptions<PendingContext> options) : base(options) { }
    }
}
