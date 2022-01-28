namespace Chronoria_WebAPI.Services
{
    public class IdService : IIdService
    {
        public string generate()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }

        public bool validate(string id)
        {
            try
            {
                Guid guid = Guid.Parse(id);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
