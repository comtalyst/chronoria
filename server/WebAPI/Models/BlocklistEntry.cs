namespace Chronoria_WebAPI.Models
{
    public class BlocklistEntry
    {
        public string Email { get; set; }
        public string Capture { get; set; }
        public DateTime Release { get; set; }
        public string Reasons { get; set; }
    }
}
