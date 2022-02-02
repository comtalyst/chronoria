using Chronoria_WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Chronoria_WebAPI.Repositories
{
    public abstract class FileContentRepository<DbContextType> : IFileContentRepository<DbContextType> where DbContextType : BaseContext
    {
        protected readonly DbContextType _context;

        public FileContentRepository(DbContextType context)
        {
            _context = context;
        }

        public async Task<FileContent> Create(FileContent entry)
        {
            _context.FileContents.Add(entry);
            await _context.SaveChangesAsync();

            return entry;
        }

        public async Task Delete(int id)
        {
            var entry = await _context.FileContents.FindAsync(id);
            _context.FileContents.Remove(entry);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<FileContent>> Get()
        {
            return _context.FileContents.ToList();
        }

        public async Task<FileContent> Get(int id)
        {
            return await _context.FileContents.FindAsync(id);
        }

        public async Task Update(FileContent entry)
        {
            _context.Entry(entry).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
