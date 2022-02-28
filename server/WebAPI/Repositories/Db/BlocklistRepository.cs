using Chronoria_WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Chronoria_WebAPI.Repositories
{
    public class BlocklistRepository : IBlocklistRepository
    {
        protected readonly MetaContext _context;

        public BlocklistRepository(MetaContext context)
        {
            _context = context;
        }
        public async Task<BlocklistEntry> Create(BlocklistEntry entry)
        {
            _context.BlocklistEntries.Add(entry);
            await _context.SaveChangesAsync();

            return entry;
        }

        public async Task Delete(string id)
        {
            var entry = await _context.BlocklistEntries.FindAsync(id);
            if (entry == null)
            {
                return;
            }
            _context.BlocklistEntries.Remove(entry);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BlocklistEntry>> Get()
        {
            return _context.BlocklistEntries.ToList();
        }

        public async Task<BlocklistEntry> Get(string id)
        {
            return await _context.BlocklistEntries.FindAsync(id);
        }

        public async Task Update(BlocklistEntry entry)
        {
            _context.Entry(entry).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
