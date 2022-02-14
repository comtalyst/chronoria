using Microsoft.EntityFrameworkCore;

namespace Chronoria_WebAPI.Models
{
    public abstract class BaseContext : DbContext
    {
        public BaseContext(DbContextOptions options) : base(options) {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Capsule>().HasKey(entry => entry.Id);
            modelBuilder.Entity<FileContent>().HasKey(entry => entry.Id);
            modelBuilder.Entity<TextContent>().HasKey(entry => entry.Id);
        }

        // Each DbSet will map to a table in the database
        public DbSet<Capsule> Capsules { get; set; }
        public DbSet<FileContent> FileContents { get; set; }
        public DbSet<TextContent> TextContents { get; set; }
    }
}
