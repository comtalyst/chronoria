using Microsoft.EntityFrameworkCore;

namespace Chronoria_WebAPI.Models
{
    public class MetaContext : DbContext
    {
        public MetaContext(DbContextOptions options) : base(options) { }

        // Each DbSet will map to a table in the database
        public DbSet<BlocklistEntry> BlocklistEntries { get; set; }
    }
}
