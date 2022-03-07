using Chronoria_ConsumerWorkers.Models;
using Microsoft.EntityFrameworkCore;

namespace Chronoria_ConsumerWorkers.Repositories
{
    public class FileContentRepository<DbContextType> : IFileContentRepository<DbContextType> where DbContextType : BaseContext
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

        public async Task Delete(string id)
        {
            try
            {
                var entry = await _context.FileContents.FindAsync(id);
                if (entry == null)
                {
                    return;
                }
                _context.FileContents.Remove(entry);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        public async Task<FileContent> Retrieve(string id)
        {
            try
            {
                var entry = await _context.FileContents.FindAsync(id);
                if (entry == null)
                {
                    return null;
                }
                _context.FileContents.Remove(entry);
                await _context.SaveChangesAsync();
                return entry;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<FileContent>> Get()
        {
            return _context.FileContents.ToList();
        }

        public async Task<FileContent> Get(string id)
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
