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
            try
            {
                var entry = await _context.Capsules.FindAsync(id);
                if (entry == null)
                {
                    return;
                }
                _context.Capsules.Remove(entry);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        public async Task<Capsule> Retrieve(string id)
        {
            try
            {
                var entry = await _context.Capsules.FindAsync(id);
                if (entry == null)
                {
                    return null;
                }
                _context.Capsules.Remove(entry);
                await _context.SaveChangesAsync();
                return entry;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
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
