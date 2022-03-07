using Microsoft.EntityFrameworkCore;

namespace Chronoria_ConsumerWorkers.Models
{
    public class ArchivedContext : BaseContext
    {
        public ArchivedContext(DbContextOptions<ArchivedContext> options) : base(options) { }
    }
}
