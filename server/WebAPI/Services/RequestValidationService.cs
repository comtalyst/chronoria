using Chronoria_WebAPI.Models;
using Chronoria_WebAPI.utils;

namespace Chronoria_WebAPI.Services
{
    public class RequestValidationService : IRequestValidationService
    {
        IConfiguration constraints;
        public RequestValidationService(IConfiguration constraints)
        {
            this.constraints = constraints;
        }
        public void ValidateEmail(string email)
        {
            if(email.Length > constraints.GetValue<int>("EmailCh"))
            {
                throw new ArgumentException();
            }
            if (email.EndsWith('.'))
            {
                throw new ArgumentException();
            }
            try
            {
                System.Diagnostics.Debug.Assert(new System.Net.Mail.MailAddress(email).Address == email);
            }
            catch
            {
                throw new ArgumentException();
            }
        }

        public void ValidateFile(UploadedFile file)
        {
            long size = file.FormFile.Length;
            if(size > constraints.GetValue<long>("FileBytes"))
            {
                throw new ArgumentException();
            }
        }

        public void ValidateFutureTime(long time)
        {
            DateTime curTime = TimeUtils.now();
            DateTime dateTime = TimeUtils.EpochMsToDateTime(time);
            if(curTime > dateTime)
            {
                throw new ArgumentException();
            }
        }

        public void ValidateName(string name)
        {
            // watch for SQL injection?
            if (name.Length > constraints.GetValue<int>("NameCh"))
            {
                throw new ArgumentException();
            }
        }

        public void ValidateText(string text)
        {
            if (text.Length > constraints.GetValue<int>("TextCh"))
            {
                throw new ArgumentException();
            }
        }

        public void ValidateTextLoc(string textLoc)
        {
            if (!Enum.TryParse(typeof(TextLocation), textLoc, out _)) 
            {
                throw new ArgumentException();
            }
        }
    }
}
