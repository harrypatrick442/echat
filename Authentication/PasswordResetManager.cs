
using Core.Exceptions;
using Authentication.DAL;
using System.Timers;
using Timer = System.Timers.Timer;
using Core.Collections;

namespace Authentication
{
    public class PasswordResetManager
    {
        private static PasswordResetManager _Instance;
        public static PasswordResetManager Instance { get {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(AuthenticationManager));
                return _Instance; } }
        public static PasswordResetManager Initialize(DalAuthentication dalAuthentication)
        {
            if (_Instance != null) throw new NotInitializedException(nameof(AuthenticationManager));
            _Instance = new PasswordResetManager(dalAuthentication);
            return _Instance;
        }
        private DalAuthentication _DalAuthentication;
        private StagedDictionaryBuffer<string, long> _MapSecretToUserId = new StagedDictionaryBuffer<string, long>(Authentication.Constants.PASSWORD_RESET_MANAGER_STAGED_DICTIONARY_N_STAGES);
        private Timer _TimerSwitch;
        private PasswordResetManager(DalAuthentication dalAuthentication)
        {
            _DalAuthentication = dalAuthentication;
            _TimerSwitch = new Timer();
            _TimerSwitch.Interval = Authentication.Constants.PASSWORD_RESET_MANAGER_STAGED_DICTIONARY_SWITCH_INTERVAL;
            _TimerSwitch.Elapsed += TimerTicked;
            _TimerSwitch.AutoReset = true;
            _TimerSwitch.Enabled = true;
            _TimerSwitch.Start();
        }
        public string Prepare(long userId) {
            string secret;
            do
            {
                secret = Guid.NewGuid().ToString("N");
            }
            while (_MapSecretToUserId.ContainsKey(secret));
            _MapSecretToUserId.AddIntoLatest(secret, userId);
            return secret;
        }
        public bool GetUserForSecret(string secret, out long userId)
        {
            return _MapSecretToUserId.TryGetValue(secret, out userId);
        }
        public bool TryToUpdatePassword(string secret, string password) {
            if (!_MapSecretToUserId.TryGetValue(secret, out long userId)) {
                return false;
            }
            DalAuthentication.Instance.UpdatePassword(userId, password);
            _MapSecretToUserId.Remove(secret);
            return true;
        }

        private void TimerTicked(object sender, ElapsedEventArgs e) {
            _MapSecretToUserId.Switch();
        }
    }
}
