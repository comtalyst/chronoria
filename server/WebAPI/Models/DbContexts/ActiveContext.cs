using Microsoft.EntityFrameworkCore;

namespace Chronoria_WebAPI.Models
{
    public class ActiveContext : BaseContext
    {
        public ActiveContext(DbContextOptions<ActiveContext> options) : base(options) { }
    }
}
