namespace Chronoria_ConsumerWorkers.Services
{
    public interface ILinkService
    {
        public string GetConfirmationLink(string id);
        public string GetDownloadLink(string id);
        public string GetCancelationLink(string id);
    }
}
