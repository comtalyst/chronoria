using Chronoria_WebAPI.Models;

namespace Chronoria_WebAPI.Services
{
    public interface IRequestValidationService
    {
        protected const string InvalidEmail = "INVALID_EMAIL";
        protected const string InvalidName = "INVALID_NAME";
        protected const string InvalidFutureTime = "INVALID_FUTURE_TIME";
        protected const string InvalidText = "INVALID_TEXT";
        protected const string InvalidTextLoc = "INVALID_TEXT_LOC";
        protected const string InvalidFile = "INVALID_FILE";
        protected const string InvalidId = "INVALID_ID";

        public void ValidateEmail(string email);
        public void ValidateName(string name);
        public void ValidateFutureTime(long time);
        public void ValidateText(string text);
        public void ValidateTextLoc(string textLoc);
        public void ValidateFile(UploadedFile file);
        public void ValidateId(string id);
    }
}
