using Microsoft.EntityFrameworkCore;

namespace Chronoria_WebAPI.Models
{
    public class PendingContext : DbContext
    {
        public PendingContext(DbContextOptions<PendingContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        // Each DbSet will map to a table in the database
        public DbSet<Capsule> Capsules { get; set; }
        public DbSet<FileContent> FileContents { get; set; }
        public DbSet<TextContent> TextContents { get; set; }
    }
}
