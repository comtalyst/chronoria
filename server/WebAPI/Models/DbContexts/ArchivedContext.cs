using Microsoft.EntityFrameworkCore;

namespace Chronoria_WebAPI.Models
{
    public class ArchivedContext : BaseContext
    {
        public ArchivedContext(DbContextOptions<ArchivedContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
