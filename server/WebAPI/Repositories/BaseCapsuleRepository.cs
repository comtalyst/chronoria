using Chronoria_WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Chronoria_WebAPI.Repositories
{
    public abstract class BaseCapsuleRepository : IBaseCapsuleRepository
    {
        protected readonly BaseContext _context;

        public BaseCapsuleRepository(BaseContext context)
        {
            _context = context;
        }

        public async Task<Capsule> Create(Capsule capsule)
        {
            _context.Capsules.Add(capsule);
            await _context.SaveChangesAsync();

            return capsule;
        }

        public async Task Delete(int id)
        {
            var capsule = await _context.Capsules.FindAsync(id);
            _context.Capsules.Remove(capsule);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Capsule>> Get()
        {
            return _context.Capsules.ToList();
        }

        public async Task<Capsule> Get(int id)
        {
            return await _context.Capsules.FindAsync(id);
        }

        public async Task Update(Capsule capsule)
        {
            _context.Entry(capsule).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
