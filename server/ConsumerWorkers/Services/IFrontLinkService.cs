namespace Chronoria_ConsumerWorkers.Services
{
    public interface IFrontLinkService
    {
        public string GetConfirmationLink(string id);
        public string GetDownloadLink(string id);
        public string GetCancelationLink(string id);
    }
}
