using Microsoft.EntityFrameworkCore;

namespace Chronoria_WebAPI.Models
{
    public class MetaContext : DbContext
    {
        public MetaContext(DbContextOptions<MetaContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlocklistEntry>().HasKey(entry => entry.Email);
        }

        // Each DbSet will map to a table in the database
        public DbSet<BlocklistEntry> BlocklistEntries { get; set; }
    }
}
