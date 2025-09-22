using GlobalConstants;
namespace EChat.Constants
{
    public class EChatCredentialsSetup
    {

        [ExportToJavaScript]
        public const bool GUEST_ENABLED = true;
        [ExportToJavaScript]
        public const bool EMAIL_REQUIRED_TO_REGISTER = true;
        [ExportToJavaScript]
        public const bool USERNAME_REQUIRED_TO_REGISTER = true;
        [ExportToJavaScript]
        public const bool PHONE_REQUIRED_TO_REGISTER = false;
        [ExportToJavaScript]
        public const bool USERNAME_UNIQUE = false;
        [ExportToJavaScript]
        public const int USERNAME_MIN_LENGTH = 1;
        [ExportToJavaScript]
        public const int USERNAME_MAX_LENGTH = 50;
        [ExportToJavaScript]
        public const int EMAIL_MAX_LENGTH = 320;
        public const int PHONE_MAX_LENGTH = 50;
        [ExportToJavaScript]
        public const int PASSWORD_MIN_LENGTH = 10;
        [ExportToJavaScript]
        public const int PASSWORD_MAX_LENGTH = 100;
        [ExportToJavaScript]
        public const bool EMAIL_PASSWORD_LOG_IN_ENABLED = true;
        [ExportToJavaScript]
        public const bool PHONE_PASSWORD_LOG_IN_ENABLED = false;
        [ExportToJavaScript]
        public const bool EMAIL_ONLY_LOG_IN_ENABLED = false;
    }
}