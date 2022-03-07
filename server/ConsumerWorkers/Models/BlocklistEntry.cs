namespace Chronoria_ConsumerWorkers.Models
{
    public class BlocklistEntry
    {
        public string Email { get; set; }
        public DateTime Capture { get; set; }
        public DateTime Release { get; set; }
        public string Reasons { get; set; }

        public BlocklistEntry(string Email, DateTime Capture, DateTime Release, string Reasons)
        {
            this.Email = Email;
            this.Capture = Capture;
            this.Release = Release;
            this.Reasons = Reasons;
        }
    }
}
