namespace Chronoria_WebAPI.Models
{
    public class RejectException : Exception
    {
        public const string InvalidEmail = "INVALID_EMAIL";
        public const string InvalidName = "INVALID_NAME";
        public const string InvalidFutureTime = "INVALID_FUTURE_TIME";
        public const string InvalidText = "INVALID_TEXT";
        public const string InvalidTextLoc = "INVALID_TEXT_LOC";
        public const string InvalidFile = "INVALID_FILE";
        public const string InvalidId = "INVALID_ID";

        public const string CapsuleNotFound = "CAPSULE_NA";
        public const string ContentNotFound = "CONTENT_NA";
        public const string TextBlobNotFound = "TEXTBLOB_NA";
        public const string FileBlobNotFound = "FILEBLOB_NA";
        public const string TransferFailed = "TRANSFER_FAILED";

        public const string BlockedSenderEmail = "BLOCKED_SENDER_EMAIL";

        public const string VerificationFailed = "VERIFICATION_FAILED";
        public RejectException(string message) : base(message) { }
    }
}
