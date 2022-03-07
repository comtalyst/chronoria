using Microsoft.EntityFrameworkCore;

namespace Chronoria_ConsumerWorkers.Models
{
    public class ActiveContext : BaseContext
    {
        public ActiveContext(DbContextOptions<ActiveContext> options) : base(options) { }
    }
}
