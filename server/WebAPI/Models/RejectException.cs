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

        public const string CapsuleNotFoundOrExpired = "CAPSULE_NA_OR_EXPIRED";
        public const string ContentNotFoundOrExpired = "CONTENT_NA_OR_EXPIRED";
        public const string TextBlobNotFoundOrExpired = "TEXTBLOB_NA_OR_EXPIRED";
        public const string FileBlobNotFoundOrExpired = "FILEBLOB_NA_OR_EXPIRED";
        public const string TransferFailedOrExpired = "TRANSFER_FAILED_OR_EXPIRED";

        public const string CapsuleNotFoundOrReleased = "CAPSULE_NA_OR_RELEASED";
        public const string ContentNotFoundOrReleased = "CONTENT_NA_OR_RELEASED";
        public const string TextBlobNotFoundOrReleased = "TEXTBLOB_NA_OR_RELEASED";
        public const string FileBlobNotFoundOrReleased = "FILEBLOB_NA_OR_RELEASED";
        public const string TransferFailedOrReleased = "TRANSFER_FAILED_OR_RELEASED";

        public const string BlockedSenderEmail = "BLOCKED_SENDER_EMAIL";

        public const string VerificationFailed = "VERIFICATION_FAILED";
        public RejectException(string message) : base(message) { }
    }
}
