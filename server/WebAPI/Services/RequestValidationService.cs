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
                throw new RejectException(RejectException.InvalidEmail);
            }
            if (email.EndsWith('.'))
            {
                throw new RejectException(RejectException.InvalidEmail);
            }
            try
            {
                System.Diagnostics.Debug.Assert(new System.Net.Mail.MailAddress(email).Address == email);
            }
            catch
            {
                throw new RejectException(RejectException.InvalidEmail);
            }
        }

        public void ValidateFile(UploadedFile file)
        {
            long size = file.FormFile.Length;
            if(size > constraints.GetValue<long>("FileBytes"))
            {
                throw new RejectException(RejectException.InvalidFile);
            }
        }

        public void ValidateFutureTime(long time)
        {
            DateTime curTime = TimeUtils.now();
            DateTime dateTime = TimeUtils.EpochMsToDateTime(time);
            if(curTime > dateTime)
            {
                throw new RejectException(RejectException.InvalidFutureTime);
            }
        }

        public void ValidateName(string name)
        {
            // watch for SQL injection?
            if (name.Length > constraints.GetValue<int>("NameCh"))
            {
                throw new RejectException(RejectException.InvalidName);
            }
        }

        public void ValidateText(string text)
        {
            if (text.Length > constraints.GetValue<int>("TextCh"))
            {
                throw new RejectException(RejectException.InvalidText);
            }
        }

        public void ValidateTextLoc(string textLoc)
        {
            if (!Enum.TryParse(typeof(TextLocation), textLoc, out _)) 
            {
                throw new RejectException(RejectException.InvalidTextLoc);
            }
        }

        public void ValidateId(string id)
        {
            try
            {
                Guid guid = Guid.Parse(id);
            }
            catch (Exception)
            {
                throw new RejectException(RejectException.InvalidId);
            }
        }
    }
}
