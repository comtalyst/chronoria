namespace Chronoria_WebAPI.Services
{
    public interface ICancelationService
    {
        public Task Cancel(string id);
    }
}
