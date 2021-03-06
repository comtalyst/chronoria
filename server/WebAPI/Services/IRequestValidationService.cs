using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Services
{
    public interface IRequestValidationService
    {
        public void ValidateEmail(string email);
        public void ValidateName(string name);
        public void ValidateFutureTime(long time);
        public void ValidateText(string text);
        public void ValidateTextLoc(string textLoc);
        public void ValidateFile(UploadedFile file);
        public void ValidateId(string id);
    }
}
