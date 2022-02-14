using Chronoria_WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Chronoria_WebAPI.Repositories
{
    public class CapsuleRepository<DbContextType> : ICapsuleRepository<DbContextType> where DbContextType : BaseContext
    {
        protected readonly DbContextType _context;

        public CapsuleRepository(DbContextType context)
        {
            _context = context;
        }

        public async Task<Capsule> Create(Capsule entry)
        {
            _context.Capsules.Add(entry);
            await _context.SaveChangesAsync();

            return entry;
        }

        public async Task Delete(string id)
        {
            var entry = await _context.Capsules.FindAsync(id);
            _context.Capsules.Remove(entry);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Capsule>> Get()
        {
            return _context.Capsules.ToList();
        }

        public async Task<Capsule> Get(string id)
        {
            return await _context.Capsules.FindAsync(id);
        }

        public async Task Update(Capsule entry)
        {
            _context.Entry(entry).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
