using Chronoria_ConsumerWorkers.Models;
using Microsoft.EntityFrameworkCore;

namespace Chronoria_ConsumerWorkers.Repositories
{
    public class TextContentRepository<DbContextType> : ITextContentRepository<DbContextType> where DbContextType : BaseContext
    {
        protected readonly DbContextType _context;

        public TextContentRepository(DbContextType context)
        {
            _context = context;
        }

        public async Task<TextContent> Create(TextContent entry)
        {
            _context.TextContents.Add(entry);
            await _context.SaveChangesAsync();

            return entry;
        }

        public async Task Delete(string id)
        {
            try
            {
                var entry = await _context.TextContents.FindAsync(id);
                if (entry == null)
                {
                    return;
                }
                _context.TextContents.Remove(entry);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        public async Task<TextContent> Retrieve(string id)
        {
            try
            {
                var entry = await _context.TextContents.FindAsync(id);
                if (entry == null)
                {
                    return null;
                }
                _context.TextContents.Remove(entry);
                await _context.SaveChangesAsync();
                return entry;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<TextContent>> Get()
        {
            return _context.TextContents.ToList();
        }

        public async Task<TextContent> Get(string id)
        {
            return await _context.TextContents.FindAsync(id);
        }

        public async Task Update(TextContent entry)
        {
            _context.Entry(entry).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
