namespace Chronoria_WebAPI.Services
{
    public interface IBlocklistService
    {
        public Task Block(string email, DateTimeOffset duration, string reasons);
        public Task Unblock(string email);
        public Task<bool> BlockExists(string email);
    }
}
