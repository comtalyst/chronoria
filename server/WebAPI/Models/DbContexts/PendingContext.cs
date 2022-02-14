using Microsoft.EntityFrameworkCore;

namespace Chronoria_WebAPI.Models
{
    public class PendingContext : BaseContext
    {
        public PendingContext(DbContextOptions<PendingContext> options) : base(options) { }
    }
}
