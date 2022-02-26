namespace Chronoria_WebAPI.Models
{
    public class RejectException : Exception
    {
        public RejectException(string message) : base(message) { }
    }
}
