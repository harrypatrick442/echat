using Core.Exceptions;
using System.Text.RegularExpressions;
using Native.Clipboards;

namespace EChatAndroid.Clipboard{
    public sealed class EChatClipboardWatcher
    {
        //private Regex _RegexIsEChatLink = new Regex("(?:echat.com|localhost:3000|10.0.2.2:3000)\\/d\\/([a-zA-Z0-9]+)");
        private static EChatClipboardWatcher? _Instance;
        public static EChatClipboardWatcher Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(EChatClipboardWatcher));
                return _Instance;
            }
        }
        public static EChatClipboardWatcher Initialize(IClipboardWatcher clipboardWatcher)
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(EChatClipboardWatcher));
            _Instance = new EChatClipboardWatcher(clipboardWatcher);
            return _Instance;
        }
        private IClipboardWatcher _ClipboardWatcher;
        private EChatClipboardWatcher(IClipboardWatcher clipboardWatcher)
        {
            _ClipboardWatcher = clipboardWatcher;
            clipboardWatcher.ClipboardContentChanged += _ContentChanged;
        }
        private void _ContentChanged(object? sender, EventArgs e) {
            string? text = _ClipboardWatcher.GetText();
            if (text == null) return;
            /*
            string token = FilesRelayTokensHelper.GetTokenFromPotentialUrl(text);
            if(token== null) return;
            FilesRelayWebViewInterface.Instance.SendGotNewToken(token);*/
        }
    }
}