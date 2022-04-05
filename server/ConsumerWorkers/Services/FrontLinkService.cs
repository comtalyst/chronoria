namespace Chronoria_ConsumerWorkers.Services
{
    public class FrontLinkService : IFrontLinkService
    {
        private readonly string domain;
        private readonly string cancelPath;
        private readonly string confirmPath;
        private readonly string downloadPath;
        public FrontLinkService(
            string domain,                  // e.g. https://timelette.app
            string cancelPath,              // e.g. /cancel
            string confirmPath,             // e.g. /confirm
            string downloadPath)            // e.g. /download
        {
            this.domain = domain;
            this.downloadPath = downloadPath;
            this.confirmPath = confirmPath;
            this.cancelPath = cancelPath;
        }

        public string GetCancelationLink(string id)
        {
            return domain + cancelPath + "/" + id;
        }

        public string GetConfirmationLink(string id)
        {
            return domain + confirmPath + "/" + id;
        }

        public string GetDownloadLink(string id)
        {
            return domain + downloadPath + "/" + id;
        }
    }
}
