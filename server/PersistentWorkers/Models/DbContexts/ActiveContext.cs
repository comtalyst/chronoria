using Microsoft.EntityFrameworkCore;

namespace Chronoria_PersistentWorkers.Models
{
    public class ActiveContext : BaseContext
    {
        public ActiveContext(DbContextOptions<ActiveContext> options) : base(options) { }
    }
}
