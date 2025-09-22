using Core.Limiters;

namespace Authentication
{
    public class CredentialsSetup
    {
        public int PasswordMinLength { get; }
        public int PasswordMaxLength { get; }
        public int UsernameMinLength { get; }
        public int UsernameMaxLength { get; }
        public bool EmailRequiredToRegister { get; }
        public bool PhoneRequiredToRegister { get; }
        public bool UsernameRequiredToRegister { get; }
        public bool EmailPasswordLogInEnabled { get; }
        public bool UsernamePasswordLogInEnabled { get; }
        public bool EmailOnlyLogInEnabled { get; }
        public bool UsernamesUnique { get; }
        public bool GuestEnabled { get; }
        public CredentialsSetup(bool guestEnabled, bool emailRequiredToRegister, bool phoneRequiredToRegister, bool usernameRequiredToRegister, 
            int passwordMinLength, int passwordMaxLength, int usernameMinLength,
            int usernameMaxLength, bool usernamesUnique,
            bool emailPasswordLogInEnabled,
            bool phonePasswordLogInEnabled,
            bool emailOnlyLogInEnabled) {
            GuestEnabled = guestEnabled;
            EmailRequiredToRegister = emailRequiredToRegister;
            PhoneRequiredToRegister = phoneRequiredToRegister;
            UsernameRequiredToRegister = usernameRequiredToRegister;
            PasswordMinLength = passwordMinLength;
            PasswordMaxLength = passwordMaxLength;  
            UsernameMinLength = usernameMinLength;  
            UsernameMaxLength = usernameMaxLength;  
            UsernamesUnique = usernamesUnique;
            EmailPasswordLogInEnabled = emailPasswordLogInEnabled;
            UsernamePasswordLogInEnabled = phonePasswordLogInEnabled;
            EmailOnlyLogInEnabled = emailOnlyLogInEnabled;
        }   
    }
}
