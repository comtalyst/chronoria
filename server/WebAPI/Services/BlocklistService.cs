using Chronoria_WebAPI.Repositories;
using Chronoria_WebAPI.Models;
using Chronoria_WebAPI.utils;

namespace Chronoria_WebAPI.Services
{
    public class BlocklistService : IBlocklistService
    {
        private IBlocklistRepository blocklistRepository;

        public BlocklistService(IBlocklistRepository blocklistRepository)
        {
            this.blocklistRepository = blocklistRepository;
        }
        public async Task Block(string email, DateTimeOffset duration, string reasons)
        {
            var time = TimeUtils.now();
            BlocklistEntry entry = new BlocklistEntry(email, time, time.AddMilliseconds(duration.ToUnixTimeMilliseconds()), reasons);
            await blocklistRepository.Create(entry);
        }

        public async Task<bool> BlockExists(string email)
        {
            if(await blocklistRepository.Get(email) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task Unblock(string email)
        {
            await blocklistRepository.Delete(email);
        }
    }
}
