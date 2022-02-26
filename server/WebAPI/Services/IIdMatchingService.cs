namespace Chronoria_WebAPI.Services
{
    public interface IIdMatchingService
    {
        public enum DbName
        {
            Pending,
            Active,
            Archived
        }
        public Task<bool> MatchReceipientEmail(string id, string receipientEmail, DbName dbName);
    }
}
