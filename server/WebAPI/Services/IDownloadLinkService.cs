namespace Chronoria_WebAPI.Services
{
    public interface IDownloadLinkService
    {
        public Task<string> GetLink(string id);
    }
}
