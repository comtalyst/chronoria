namespace Chronoria_WebAPI.Models
{
    public class ConfEmailMessage : IMessage
    {
        public string Email { get; set; }
        public string Ref { get; set; }

        public ConfEmailMessage(string Email, string Ref)
        {
            this.Email = Email;
            this.Ref = Ref;
        }
        public string Jsonify()
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
