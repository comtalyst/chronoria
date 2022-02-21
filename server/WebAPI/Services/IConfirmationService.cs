namespace Chronoria_WebAPI.Services
{
    public interface IConfirmationService
    {
        public Task Confirm(string id);
    }
}
