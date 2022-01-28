namespace Chronoria_WebAPI.Services
{
    public interface IIdService
    {
        public string generate();
        public bool validate(string id);
    }
}
